using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ZionAdmin.Models.LuceneSearch
{

    public static class LuceneSearch
    {
        static int secondChance = 0;

        private static string _luceneDir = System.IO.Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                try
                {
                    if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                    if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                    var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                    if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                    return _directoryTemp;
                }
                catch (Exception)
                {
                    return _directoryTemp;
                }
                
            }
        }


        private static void _addToLuceneIndex(LuceneReport sampleData, IndexWriter writer)
        {

            // remove older index entry
            var searchQuery = new Term[] { new Term("ReportId", sampleData.ReportId.ToString()), new Term("ReportUrl", sampleData.ReportUrl) };
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("ReportId", sampleData.ReportId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("ReportTitle", sampleData.ReportTitle, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Category", sampleData.Category, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("ReportUrl", sampleData.ReportUrl, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            //doc.Add(new Field("ReportDescription", sampleData.ReportDescription, Field.Store.YES, Field.Index.ANALYZED));

            // add entry to index
            writer.AddDocument(doc);

        }
        public static void AddUpdateLuceneIndex(IEnumerable<LuceneReport> sampleDatas)
        {
            try
            {
                // init lucene
                using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                {
                    using (var writer = new IndexWriter(LuceneSearch._directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                    {
                        // add data to lucene search index (replaces older entry if any)
                        foreach (var sampleData in sampleDatas) lock (sampleData) { _addToLuceneIndex(sampleData, writer); }
                        // close handles
                        //analyzer.Close();
                        //writer.Dispose();
                    }
                }
            }
            catch
            {
                //clear the lock
                _directory.ClearLock("write.lock");
                if (secondChance == 0)
                {
                    secondChance++;
                    AddUpdateLuceneIndex(sampleDatas);
                }
            }
        }

        public static void AddUpdateLuceneIndex(LuceneReport sampleData)
        {
            lock (sampleData)
            {
                AddUpdateLuceneIndex(new List<LuceneReport> { sampleData });
            }
        }

        public static void ClearLuceneIndexRecord(int record_id)
        {
            // init lucene
            using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            {
                using (var writer = new IndexWriter(LuceneSearch._directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entry
                    var searchQuery = new TermQuery(new Term("ReportId", record_id.ToString()));
                    writer.DeleteDocuments(searchQuery);
                }
            }
        }

        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                using (var writer = new IndexWriter(LuceneSearch._directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    // writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(LuceneSearch._directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        private static LuceneReport _mapLuceneDocumentToData(Document doc)
        {
            return new LuceneReport
            {
                ReportId = Convert.ToInt32(doc.Get("ReportId")),
                ReportTitle = doc.Get("ReportTitle"),
                Category = doc.Get("Category"),
                ReportUrl = doc.Get("ReportUrl")
            };
        }

        private static IEnumerable<LuceneReport> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<LuceneReport> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static IEnumerable<LuceneReport> _mapDocumentToList(List<Document> hits)
        {
            return from h in hits
                   select new LuceneReport
                   {
                       ReportId = Convert.ToInt32(h.GetField("ReportId").StringValue),
                       ReportTitle = h.GetField("ReportTitle").StringValue,
                       ReportUrl = h.GetField("ReportUrl").StringValue,
                       Category = h.GetField("Category").StringValue
                   };
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Field != null && parser.Field == "ReportUrl" ? new TermQuery(new Term(parser.Field, searchQuery)) : parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }

        private static FuzzyQuery fparseQuery(string searchQuery)
        {
            var query = new FuzzyQuery(new Term("ReportTitle", searchQuery), 0.2f);
            return query;
        }

        private static IEnumerable<LuceneReport> _search(string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<LuceneReport>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, true))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Lucene.Net.Util.Version.LUCENE_30, new[] { "ReportId", "ReportTitle" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search
                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }

        private static ScoreDoc[] _phraseQuery(string searchQuery, out int totalRecord)
        {
            string[] searchfields = new string[] { "ReportTitle", "Category" };

            // Build our booleanquery that will be a combination of all the queries for each individual search term
            var finalQuery = new BooleanQuery();
            var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, searchfields, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            finalQuery.Add(parser.Parse(searchQuery), Occur.MUST);
            // Perform the search
            var searcher = new IndexSearcher(_directory, false);
            var hits = searcher.Search(finalQuery, 1000).ScoreDocs;
            totalRecord = hits.Count();
            return hits;

        }

        private static IEnumerable<LuceneReport> _searchFuzzy(string searchQuery, out int totalRecord, int first = 0, int last = 0)
        {
            // Setup the fields to search through
            string[] searchfields = new string[] { "ReportTitle", "Category" };

            // Build our booleanquery that will be a combination of all the queries for each individual search term
            var finalQuery = new BooleanQuery();
            var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, searchfields, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            var phraseHits = _phraseQuery(searchQuery, out totalRecord);
            searchQuery = searchQuery.Replace(":", " ");
            searchQuery = searchQuery.Replace("-", " ");
            // Split the search string into separate search terms by word
            string[] terms = searchQuery.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string term in terms)
                finalQuery.Add(parser.Parse(term.Replace("~", "") + "~"), Occur.MUST);

            // Perform the search
            var searcher = new IndexSearcher(_directory, false);

            var hits = searcher.Search(finalQuery, 1000).ScoreDocs;
            totalRecord = hits.Count();
            if (last > 0)
            {
                List<Document> results = new List<Document>();
                for (int i = first; i < last && i < hits.Count(); i++)
                {
                    results.Add(searcher.Doc(hits[i].Doc));
                }
                return _mapDocumentToList(results);
            }
            return _mapLuceneToDataList(hits, searcher);
        }

        public static IEnumerable<LuceneReport> Search(string input, out int totalRecord, string fieldName = "", int first = 0, int last = 0)
        {
            totalRecord = 0;
            if (string.IsNullOrEmpty(input)) return new List<LuceneReport>();

            //var terms = input.Trim().Replace("-", " ").Split(' ')
            //    .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            //input = string.Join(" ", terms);

            //var res = _search(input, fieldName);
            //if (res == null || res.Count() == 0)
            var res = _searchFuzzy(input, out totalRecord, first, last);
            return res;
        }

        public static IEnumerable<LuceneReport> SearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<LuceneReport>() : _search(input, fieldName);
        }

        public static IEnumerable<LuceneReport> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<LuceneReport>();

            // set up lucene searcher
            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }

        public static void DeleteLuceneIndex(LuceneReport sampleData)
        {
            using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            {

                using (var writer = new IndexWriter(LuceneSearch._directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    try
                    {
                        //var searchQuery = new TermQuery(new Term("ReportId", sampleData.ReportId.ToString()));
                        var searchQuery = new Term[] { new Term("ReportId", sampleData.ReportId.ToString()) };
                        writer.DeleteDocuments(searchQuery);
                    }
                    catch (Exception ex) { }
                    finally
                    {
                        if (writer != null)
                        {
                            writer.Optimize();
                            writer.Commit();
                            writer.Dispose();
                        }

                        if (analyzer != null)
                        {
                            analyzer.Close();
                            analyzer.Dispose();
                        }
                    }
                }
                //analyzer.Close();
            }
        }

        public static IEnumerable<LuceneReport> ShowAllUrl()
        {
            var rec = GetAllIndexRecords();
            return rec;
        }
    }
}