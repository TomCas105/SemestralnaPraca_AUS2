using SP_AUS_Lib.Structures;
using System.Diagnostics;

namespace SP_AUS_Lib.Tests
{
    public class TreeTests
    {
        public static void AVLTreeTest()
        {
            int _maxValues = 100_000;
            int _randomMax = int.MaxValue;
            int _numberOfTests = 1000;
            int _testsPassed = 0;
            Random _random = new();

            _randomMax = Math.Max(_randomMax, _maxValues);

            for (int i = 0; i < _numberOfTests; i++)
            {
                AVLTree<int, int> _tree = new();
                int[] _values = new int[_maxValues];
                int _valuesCnt = 0;

                while (_valuesCnt < _maxValues && _randomMax > _maxValues)
                {
                    int _r = _random.Next() % _randomMax;

                    if (_tree.Insert(_r, _r))
                    {
                        _values[_valuesCnt++] = _r;
                    }

                }

                int _deleteCnt = _random.Next() % (_maxValues / 2);
                for (int j = 0; j < _deleteCnt && _valuesCnt > 0; j++)
                {
                    int _index = _random.Next() % _valuesCnt;
                    int _r = _values[_index];
                    _values[_index] = _values[(_valuesCnt--) - 1];
                    _tree.Delete(_r);
                }

                if (_tree.AVLTreeTest())
                {
                    _testsPassed++;
                }
                Console.WriteLine($"AVL Tree tests passed: {_testsPassed}/{_numberOfTests}");
            }

        }

        public static void InOrderTest()
        {

            int _maxValues = 100;
            int _randomMax = int.MaxValue;
            int _numberOfTests = 100_000;
            int _testsPassed = 0;

            AVLTree<int, int> _tree = new();
            Random _random = new();

            //List obsahujúci vložené vrcholy
            List<int> _valuesInserted = new();

            //List, ktorý bude obsahovať nevložené alebo odstránené vrcholy zo stromu
            List<int> _valuesToInsert = new();

            while (_valuesToInsert.Count < _maxValues)
            {
                int _r = _random.Next() % _randomMax;
                if (!_valuesToInsert.Contains(_r))
                {
                    _valuesToInsert.Add(_r);
                }
            }

            //List pre kontrolu poradia vrcholov
            List<int> _sortedValues = new(_valuesToInsert);
            _sortedValues.Sort();

            for (int i = 0; i < _numberOfTests; i++)
            {
                //INSERT
                while (_valuesToInsert.Count > 0)
                {
                    int _r = _valuesToInsert[_random.Next() % _valuesToInsert.Count];

                    if (_tree.Insert(_r, _r))
                    {
                        _valuesInserted.Add(_r);
                        _valuesToInsert.Remove(_r);
                    }
                }

                //TEST
                var _inOrderList = _tree.InOrderTraversal();
                bool _testPassed = true;
                if (_inOrderList != null)
                {
                    for (int j = 0; j < _sortedValues.Count; j++)
                    {
                        if (_sortedValues[j] != _inOrderList[j])
                        {
                            _testPassed = false;
                            break;
                        }
                    }
                }
                else
                {
                    _testPassed = false;
                }
                if (_testPassed)
                {
                    _testsPassed++;
                }

                //Výpis inOrder zoznamu a kontrolného zoznamu
                if (_inOrderList != null && _sortedValues.Count < 50)
                {
                    foreach (var _val in _inOrderList)
                    {
                        Console.Write(_val + " ");
                    }
                    Console.WriteLine();
                    foreach (var _val in _sortedValues)
                    {
                        Console.Write(_val + " ");
                    }
                    Console.WriteLine();
                }

                //DELETE
                int _deleteCnt = _random.Next() % (_maxValues - _valuesToInsert.Count);
                for (int j = 0; j < _deleteCnt; j++)
                {
                    int r = _valuesInserted[_random.Next() % _valuesInserted.Count];

                    if (_tree.Delete(r))
                    {
                        _valuesInserted.Remove(r);
                        _valuesToInsert.Add(r);
                    }
                }
                Console.WriteLine($"InOrder tests passed: {_testsPassed}/{_numberOfTests}");
            }

        }

        public static void SpeedTest()
        {
            int _insertCnt = 10_000_000;
            int _deleteCnt = 2_000_000;
            int _findCnt = 5_000_000;
            int _findIntervalCnt = 1_000_000;
            int _minCnt = 2_000_000;
            int _maxCnt = 2_000_000;
            int _randomMax = int.MaxValue;

            AVLTree<int, int> _tree = new();
            Random _random = new();
            int[] _values = new int[_insertCnt];
            int _valuesCnt = 0;

            //INSERT
            Stopwatch _sw = Stopwatch.StartNew();
            if (_insertCnt < _randomMax)
            {
                while (_valuesCnt < _insertCnt)
                {
                    int _r = _random.Next() % _randomMax;

                    if (_tree.Insert(_r, _r))
                    {
                        _values[_valuesCnt++] = _r;
                    }
                }
            }
            _sw.Stop();
            Console.WriteLine($"Insert x{_insertCnt:n0}: " + _sw.Elapsed.ToString());
            Console.WriteLine($"Number of nodes: {_tree.Count:n0}");

            //Náhodne usporiadanie hodnôt pre mazanie
            for (int i = 0; i < _valuesCnt; i++)
            {
                var _tmp = _values[i];
                var _other = _random.Next() % _valuesCnt;
                _values[i] = _values[_other];
                _values[_other] = _tmp;
            }

            //DELETE
            if (_valuesCnt > 0)
            {
                _sw.Restart();
                for (int i = 0; i < _deleteCnt && _valuesCnt > 0; i++)
                {
                    int _r = _values[(--_valuesCnt)];
                    _tree.Delete(_r);
                }
                _sw.Stop();
                Console.WriteLine($"Delete x{_deleteCnt:n0}: " + _sw.Elapsed.ToString());
                Console.WriteLine($"Number of nodes: {_tree.Count:n0}");
            }

            //FIND
            if (_valuesCnt > 0)
            {
                _sw.Restart();
                for (int i = 0; i < _findCnt; i++)
                {
                    int _r = _values[_random.Next() % _valuesCnt];
                    _tree.Find(_r, out _);
                }
                _sw.Stop();
                Console.WriteLine($"Find x{_findCnt:n0}: " + _sw.Elapsed.ToString());
            }

            //MINIMUM
            if (_valuesCnt > 0)
            {
                _sw.Restart();
                for (int i = 0; i < _findCnt; i++)
                {
                    int _r = _values[_random.Next() % _valuesCnt];
                    _tree.Min();
                }
                _sw.Stop();
                Console.WriteLine($"Minimum x{_minCnt:n0}: " + _sw.Elapsed.ToString());
            }

            //MAXIMUM
            if (_valuesCnt > 0)
            {
                _sw.Restart();
                for (int i = 0; i < _findCnt; i++)
                {
                    int _r = _values[_random.Next() % _valuesCnt];
                    _tree.Max();
                }
                _sw.Stop();
                Console.WriteLine($"Maximum x{_maxCnt:n0}: " + _sw.Elapsed.ToString());
            }

            //FIND INTERVAL
            float _intervalWidth = (float)_randomMax / _insertCnt * 500f;
            if (_valuesCnt > 0)
            {
                _sw.Restart();
                for (int i = 0; i < _findIntervalCnt; i++)
                {
                    int _lowerBound = _random.Next() % (_randomMax - (int)_intervalWidth);
                    var _inOrderList = _tree.Find(_lowerBound, _lowerBound + (int)_intervalWidth);

                    if (_inOrderList == null || _inOrderList.Count < 500)
                    {
                        _intervalWidth *= 1.2f; //Rozšírenie intervalu ak sa nájde málo hodnôt
                    }
                }
                _sw.Stop();
                Console.WriteLine($"Find Interval x{_findIntervalCnt:n0}: " + _sw.Elapsed.ToString());
            }
        }

        public static void GeneralTest()
        {
            int _maxTime = 20; //seconds
            int _randomMax = 10_000;

            AVLTree<int, string> _tree = new();
            Random _random = new();
            Stopwatch _sw = Stopwatch.StartNew();

            int _operationsCnt = 0;
            int _insertCnt = 0;
            int _duplicatesCnt = 0;
            int _deleteCnt = 0;
            int _deleteNotFoundCnt = 0;
            int _findCnt = 0;
            int _notFoundCnt = 0;
            int _minCnt = 0;
            int _maxCnt = 0;

            int _secLast = 0;

            while (_sw.Elapsed.TotalSeconds < _maxTime)
            {
                if (_secLast != (int)_sw.Elapsed.TotalSeconds)
                {
                    Console.WriteLine($"Time: {((int)_sw.Elapsed.TotalSeconds)}/{_maxTime}s");
                }
                _secLast = (int)_sw.Elapsed.TotalSeconds;

                int _r = _random.Next() % _randomMax;
                double _p = _random.NextDouble();

                if (_p < 0.5)
                {
                    if (_tree.Insert(_r, $"{_r}"))
                    {
                        _insertCnt++;
                    }
                    else
                    {
                        _duplicatesCnt++;
                    }
                }
                else if (_p < 0.85)
                {
                    if (_tree.Delete(_r))
                    {
                        _deleteCnt++;
                    }
                    else
                    {
                        _deleteNotFoundCnt++;
                    }
                }
                else if (_p < 0.95)
                {
                    if (_tree.Find(_r, out _))
                    {
                        _findCnt++;
                    }
                    else
                    {
                        _notFoundCnt++;
                    }
                }
                else
                {
                    if (_random.NextDouble() < 0.5)
                    {
                        _tree.Min();
                        _minCnt++;
                    }
                    else
                    {
                        _tree.Max();
                        _maxCnt++;
                    }
                }
                _operationsCnt++;
            }
            _sw.Stop();
            Console.WriteLine($"Test time: {_sw.Elapsed}" +
                $"\nNumber of operations: {_operationsCnt:n0}" +
                $"\nINSERT" +
                $"\nSuccessful insertions: {_insertCnt:n0}" +
                $"\nDuplicates: {_duplicatesCnt:n0}" +
                $"\nDELETE" +
                $"\nSuccessful deletions: {_deleteCnt:n0}" +
                $"\nNumber of deleted values not found: {_deleteNotFoundCnt:n0}" +
                $"\nFIND" +
                $"\nNumber of values found: {_findCnt:n0}" +
                $"\nNumber of values not found: {_notFoundCnt:n0}" +
                $"\nMINIMUM" +
                $"\nNumber of minimum values found: {_minCnt:n0}" +
                $"\nMAXIMUM" +
                $"\nNumber of maximum values found: {_maxCnt:n0}");
        }

        public static void ComparisonTest()
        {
            int _maxTime = 120; //seconds
            int _maxValues = 100_000;
            int _randomMax = int.MaxValue;

            BinarySearchTree<int, int> _BSTree = new();
            AVLTree<int, int> _AVLTree = new();
            List<int> _list = new(1);
            LinkedList<int> _linkedList = new();
            SortedList<int, int> _sortedList = new(1);
            Dictionary<int, int> _dictionary = new(1);

            Random _random = new();

            int _operationsCnt = 0;
            int _insertCnt = 0;
            int _deleteCnt = 0;
            int _findCnt = 0;
            int _findIntervalCnt = 0;
            int _minCnt = 0;
            int _maxCnt = 0;

            List<int> _keysInserted = new();

            Stopwatch _sw = Stopwatch.StartNew();

            Stopwatch _swBST_insert = Stopwatch.StartNew();
            Stopwatch _swAVLT_insert = Stopwatch.StartNew();
            Stopwatch _swList_insert = Stopwatch.StartNew();
            Stopwatch _swLList_insert = Stopwatch.StartNew();
            Stopwatch _swSList_insert = Stopwatch.StartNew();
            Stopwatch _swDict_insert = Stopwatch.StartNew();

            Stopwatch _swBST_delete = Stopwatch.StartNew();
            Stopwatch _swAVLT_delete = Stopwatch.StartNew();
            Stopwatch _swList_delete = Stopwatch.StartNew();
            Stopwatch _swLList_delete = Stopwatch.StartNew();
            Stopwatch _swSList_delete = Stopwatch.StartNew();
            Stopwatch _swDict_delete = Stopwatch.StartNew();

            Stopwatch _swBST_find = Stopwatch.StartNew();
            Stopwatch _swAVLT_find = Stopwatch.StartNew();
            Stopwatch _swList_find = Stopwatch.StartNew();
            Stopwatch _swLList_find = Stopwatch.StartNew();
            Stopwatch _swSList_find = Stopwatch.StartNew();
            Stopwatch _swDict_find = Stopwatch.StartNew();

            Stopwatch _swBST_findInterval = Stopwatch.StartNew();
            Stopwatch _swAVLT_findInterval = Stopwatch.StartNew();
            Stopwatch _swList_findInterval = Stopwatch.StartNew();
            Stopwatch _swLList_findInterval = Stopwatch.StartNew();
            Stopwatch _swSList_findInterval = Stopwatch.StartNew();
            Stopwatch _swDict_findInterval = Stopwatch.StartNew();

            Stopwatch _swBST_min = Stopwatch.StartNew();
            Stopwatch _swAVLT_min = Stopwatch.StartNew();
            Stopwatch _swList_min = Stopwatch.StartNew();
            Stopwatch _swLList_min = Stopwatch.StartNew();
            Stopwatch _swSList_min = Stopwatch.StartNew();
            Stopwatch _swDict_min = Stopwatch.StartNew();

            Stopwatch _swBST_max = Stopwatch.StartNew();
            Stopwatch _swAVLT_max = Stopwatch.StartNew();
            Stopwatch _swList_max = Stopwatch.StartNew();
            Stopwatch _swLList_max = Stopwatch.StartNew();
            Stopwatch _swSList_max = Stopwatch.StartNew();
            Stopwatch _swDict_max = Stopwatch.StartNew();

            _swBST_insert.Stop();
            _swAVLT_insert.Stop();
            _swList_insert.Stop();
            _swLList_insert.Stop();
            _swSList_insert.Stop();
            _swDict_insert.Stop();

            _swBST_delete.Stop();
            _swAVLT_delete.Stop();
            _swList_delete.Stop();
            _swLList_delete.Stop();
            _swSList_delete.Stop();
            _swDict_delete.Stop();

            _swBST_find.Stop();
            _swAVLT_find.Stop();
            _swList_find.Stop();
            _swLList_find.Stop();
            _swSList_find.Stop();
            _swDict_find.Stop();

            _swBST_findInterval.Stop();
            _swAVLT_findInterval.Stop();
            _swList_findInterval.Stop();
            _swLList_findInterval.Stop();
            _swSList_findInterval.Stop();
            _swDict_findInterval.Stop();

            _swBST_min.Stop();
            _swAVLT_min.Stop();
            _swList_min.Stop();
            _swLList_min.Stop();
            _swSList_min.Stop();
            _swDict_min.Stop();

            _swBST_max.Stop();
            _swAVLT_max.Stop();
            _swList_max.Stop();
            _swLList_max.Stop();
            _swSList_max.Stop();
            _swDict_max.Stop();

            while (_keysInserted.Count < _maxValues)
            {
                int _r = _random.Next() % _randomMax;
                bool _result = _keysInserted.Contains(_r);

                if (!_result)
                {
                    _BSTree.Insert(_r, _r);

                    _AVLTree.Insert(_r, _r);

                    _list.Add(_r);

                    _linkedList.AddLast(_r);

                    _sortedList.Add(_r, _r);

                    _dictionary.Add(_r, _r);

                    _keysInserted.Add(_r);
                }
            }

            int _secLast = 0;

            while (_sw.Elapsed.TotalSeconds < _maxTime)
            {
                if (_secLast != (int)_sw.Elapsed.TotalSeconds)
                {
                    Console.WriteLine($"Time: {((int)_sw.Elapsed.TotalSeconds)}/{_maxTime}s");
                }
                _secLast = (int)_sw.Elapsed.TotalSeconds;

                double _p = _random.NextDouble();

                if ((_p < 0.45 || _keysInserted.Count == 0) && _keysInserted.Count < _maxValues)
                {
                    _insertCnt++;
                    int _r = _random.Next() % _randomMax;
                    bool _result = _keysInserted.Contains(_r);

                    if (!_result)
                    {
                        _swBST_insert.Start();
                        _BSTree.Insert(_r, _r);
                        _swBST_insert.Stop();

                        _swAVLT_insert.Start();
                        _AVLTree.Insert(_r, _r);
                        _swAVLT_insert.Stop();

                        _swList_insert.Start();
                        _list.Add(_r);
                        _swList_insert.Stop();

                        _swLList_insert.Start();
                        _linkedList.AddLast(_r);
                        _swLList_insert.Stop();

                        _swSList_insert.Start();
                        _sortedList.Add(_r, _r);
                        _swSList_insert.Stop();

                        _swDict_insert.Start();
                        _dictionary.Add(_r, _r);
                        _swDict_insert.Stop();

                        _keysInserted.Add(_r);
                    }
                }
                else if (_p < 0.8)
                {
                    _deleteCnt++;
                    if (_keysInserted.Count > 0)
                    {
                        int _r = _keysInserted[_random.Next() % _keysInserted.Count];
                        bool _result = _keysInserted.Contains(_r);

                        if (_result)
                        {
                            _swBST_delete.Start();
                            _BSTree.Delete(_r);
                            _swBST_delete.Stop();

                            _swAVLT_delete.Start();
                            _AVLTree.Delete(_r);
                            _swAVLT_delete.Stop();

                            _swList_delete.Start();
                            _list.Remove(_r);
                            _swList_delete.Stop();

                            _swLList_delete.Start();
                            _linkedList.Remove(_r);
                            _swLList_delete.Stop();

                            _swSList_delete.Start();
                            _sortedList.Remove(_r);
                            _swSList_delete.Stop();

                            _swDict_delete.Start();
                            _dictionary.Remove(_r);
                            _swDict_delete.Stop();

                            _keysInserted.Remove(_r);
                        }
                    }
                }
                else if (_p < 0.92)
                {
                    _findCnt++;
                    int _r = _keysInserted[_random.Next() % _keysInserted.Count];
                    bool _result = _keysInserted.Contains(_r);

                    if (_result)
                    {
                        int? _i;

                        _swBST_find.Start();
                        _i = _BSTree[_r];
                        _swBST_find.Stop();

                        _swAVLT_find.Start();
                        _i = _AVLTree[_r];
                        _swAVLT_find.Stop();

                        _swList_find.Start();
                        _i = _list.Find((_val) => (_val == _r));
                        _swList_find.Stop();

                        _swLList_find.Start();
                        _i = _linkedList.Find(_r)?.Value;
                        _swLList_find.Stop();

                        _swSList_find.Start();
                        _i = _sortedList[_r];
                        _swSList_find.Stop();

                        _swDict_find.Start();
                        _i = _dictionary[_r];
                        _swDict_find.Stop();
                    }
                }
                else if (_p < 0.99)
                {
                    if (_random.NextDouble() < 0.5)
                    {
                        _minCnt++;

                        _swBST_min.Start();
                        _BSTree.Min();
                        _swBST_min.Stop();

                        _swAVLT_min.Start();
                        _AVLTree.Min();
                        _swAVLT_min.Stop();

                        _swList_min.Start();
                        _list.Min();
                        _swList_min.Stop();

                        _swLList_min.Start();
                        _linkedList.Min();
                        _swLList_min.Stop();

                        _swSList_min.Start();
                        _sortedList.First();
                        _swSList_min.Stop();

                        _swDict_min.Start();
                        _dictionary.Min((x) => (x.Value));
                        _swDict_min.Stop();

                    }
                    else
                    {
                        _maxCnt++;

                        _swBST_max.Start();
                        _BSTree.Max();
                        _swBST_max.Stop();

                        _swAVLT_max.Start();
                        _AVLTree.Min();
                        _swAVLT_max.Stop();

                        _swList_max.Start();
                        _list.Max();
                        _swList_max.Stop();

                        _swLList_max.Start();
                        _linkedList.Max();
                        _swLList_max.Stop();

                        _swSList_max.Start();
                        _sortedList.Last();
                        _swSList_max.Stop();

                        _swDict_max.Start();
                        _dictionary.Max((x) => (x.Value));
                        _swDict_max.Stop();
                    }
                }
                else
                {
                    _findIntervalCnt++;

                    int _w = (int)(_random.Next() % _randomMax / 10f);
                    int _l = _random.Next() % (_keysInserted.Count - _w);

                    _swBST_findInterval.Start();
                    _BSTree.Find(_l, _l + _w);
                    _swBST_findInterval.Stop();

                    _swAVLT_findInterval.Start();
                    _AVLTree.Find(_l, _l + _w);
                    _swAVLT_findInterval.Stop();

                    _swList_findInterval.Start();
                    _list.FindAll((_val) => (_val.CompareTo(_l) >= 0 && _val.CompareTo(_l + _w) <= 0));
                    _swList_findInterval.Stop();

                    _swLList_findInterval.Start();
                    _linkedList.Where((_val) => (_val.CompareTo(_l) >= 0 && _val.CompareTo(_l + _w) <= 0)).ToList();
                    _swLList_findInterval.Stop();

                    _swSList_findInterval.Start();
                    _sortedList.Where((_val) => (_val.Key >= _l && _val.Key <= _l + _w)).ToList();
                    _swSList_findInterval.Stop();

                    _swDict_findInterval.Start();
                    _dictionary.Where((_val) => (_val.Key >= _l && _val.Key <= _l + _w)).ToList();
                    _swDict_findInterval.Stop();
                }
                _operationsCnt++;
            }
            _sw.Stop();
            Console.WriteLine($"Test time: {_sw.Elapsed}" +
                $"\nNumber of operations: {_operationsCnt:n0}" +
                $"\nInsert: {_insertCnt:n0}" +
                $"\nDelete: {_deleteCnt:n0}" +
                $"\nFind: {_findCnt:n0}" +
                $"\nFind Interval: {_findIntervalCnt:n0}" +
                $"\nMinimum: {_minCnt}" +
                $"\nMaximum: {_maxCnt}" +
                $"\n\nBST" +
                $"\nInsert: {_swBST_insert.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _insertCnt)}μs" +
                $"\nDelete: {_swBST_delete.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _deleteCnt)}μs" +
                $"\nFind: {_swBST_find.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findCnt)}μs" +
                $"\nFind Interval: {_swBST_findInterval.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findIntervalCnt)}μs" +
                $"\nMinimum: {_swBST_min.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _minCnt)}μs" +
                $"\nMaximum: {_swBST_max.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _maxCnt)}μs" +
                $"\n\nAVL" +
                $"\nInsert: {_swAVLT_insert.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _insertCnt)}μs" +
                $"\nDelete: {_swAVLT_delete.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _deleteCnt)}μs" +
                $"\nFind: {_swAVLT_find.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findCnt)}μs" +
                $"\nFind Interval: {_swAVLT_findInterval.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findIntervalCnt)}μs" +
                $"\nMinimum: {_swAVLT_min.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _minCnt)}μs" +
                $"\nMaximum: {_swAVLT_max.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _maxCnt)}μs" +
                $"\n\nList" +
                $"\nInsert: {_swList_insert.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _insertCnt)}μs" +
                $"\nDelete: {_swList_delete.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _deleteCnt)}μs" +
                $"\nFind: {_swList_find.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findCnt)}μs" +
                $"\nFind Interval: {_swList_findInterval.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findIntervalCnt)}μs" +
                $"\nMinimum: {_swList_min.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _minCnt)}μs" +
                $"\nMaximum: {_swList_max.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _maxCnt)}μs" +
                $"\n\nLinkedList" +
                $"\nInsert: {_swLList_insert.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _insertCnt)}μs" +
                $"\nDelete: {_swLList_delete.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _deleteCnt)}μs" +
                $"\nFind: {_swLList_find.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findCnt)}μs" +
                $"\nFind Interval: {_swLList_findInterval.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findIntervalCnt)}μs" +
                $"\nMinimum: {_swLList_min.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _minCnt)}μs" +
                $"\nMaximum: {_swLList_max.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _maxCnt)}μs" +
                $"\n\nSortedList" +
                $"\nInsert: {_swSList_insert.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _insertCnt)}μs" +
                $"\nDelete: {_swSList_delete.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _deleteCnt)}μs" +
                $"\nFind: {_swSList_find.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findCnt)}μs" +
                $"\nFind Interval: {_swSList_findInterval.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findIntervalCnt)}μs" +
                $"\nMinimum: {_swSList_min.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _minCnt)}μs" +
                $"\nMaximum: {_swSList_max.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _maxCnt)}μs" +
                $"\n\nDictionary" +
                $"\nInsert: {_swDict_insert.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _insertCnt)}μs" +
                $"\nDelete: {_swDict_delete.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _deleteCnt)}μs" +
                $"\nFind: {_swDict_find.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findCnt)}μs" +
                $"\nFind Interval: {_swDict_findInterval.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _findIntervalCnt)}μs" +
                $"\nMinimum: {_swDict_min.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _minCnt)}μs" +
                $"\nMaximum: {_swDict_max.Elapsed.TotalSeconds * 1_000_000 / Math.Max(1.0, _maxCnt)}μs");
        }
    }
}
