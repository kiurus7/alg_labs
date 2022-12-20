using System;
using System.Collections.Generic;

namespace src
{
    public class Node<T>
    {
        public Node(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
        public Node<T> Next { get; set; }
    }

    public class NodeStack<T>
    {
        Node<T> head;
        int count = 0;
        public bool IsEmpty
        {
            get { return head == null; }
        }
        public void Push(T item)
        {
            Node<T> node = new Node<T>(item);
            node.Next = head;
            head = node;
            count++;
        }
        public T Pop()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Стек пуст");
            Node<T> temp = head;
            head = head.Next;
            count--;
            return temp.Data;

        }
        public T Peek()
        {
            if (IsEmpty)
                return default(T);
            return head.Data;
        }
        public int Count()
        {
            return count;
        }
    }

    public class CopyArray<T>
    {
        public static double coefficient = 1.5;
        public static T[] CreateNewArrayWithCopy(T[] old_array)
        {
            T[] new_array = new T[(int)(old_array.Length * coefficient)];
            for (int i = 0; i < old_array.Length; i++)
            {
                new_array[i] = old_array[i];
            }
            return new_array;
        }
    }
    public class DynamicArray<T>
    {
        private int default_size = 10;
        private T[] values;
        private int size = 0;
        public DynamicArray(int capacity)
        {
            values = new T[capacity];
        }
        public DynamicArray()
        {
            values = new T[default_size];
        }
        public int Size { get { return size; } }
        public bool isEmpty { get { return size == 0; } }
        public bool Add(T data)
        {
            if (size == values.Length)
            {
                values = CopyArray<T>.CreateNewArrayWithCopy(values);
            }
            values[size] = data;
            size++;
            return true;
        }
        public T Remove(int index)
        {
            if (index < 0 || index >= size)
                throw new InvalidOperationException("Выход за пределы массива");

            T result = values[index];
            for (int i = index; i < size - 1; i++)
            {
                values[i] = values[i + 1];
            }
            size--;
            return result;
        }
        public T get(int index)
        {
            if (index < 0 || index >= size)
                throw new InvalidOperationException("Выход за пределы массива");
            return values[index];
        }
        public T set(int index, T data)
        {
            if (index < 0 || index >= size)
                throw new InvalidOperationException("Выход за пределы массива");

            values[index] = data;
            return data;
        }
        public int indexOf(T data)
        {
            for (int i = 0; i < size; i++)
            {
                if (values[i].Equals(data))
                    return i;
            }
            return -1;
        }
        public bool Contains(T data)
        {
            if (indexOf(data) != -1)
            {
                return true;
            }
            else return false;
        }
        public void Clear()
        {
            size = 0;
            values = new T[default_size];
        }
    }
    public static class TimSort
    {
        public const int GallopSize = 7; // размер галопа

        private static int GetMinrun(int n) //метод для получения минимального Runa
        {
            int r = 0; 

            while (n >= 64) //n размер массива 
            {
                r |= n & 1;
                n >>= 1;
            }

            return n + r;
        }

        public static void DoTimSort<T>(ref DynamicArray<T> array, IComparer<T> comparer) // функция тимсорта с входным массивом 
        {
            NodeStack<(int StartIndex, int Size)> runs = new NodeStack<(int StartIndex, int Size)>(); // создаем стек Runов

            int minrun = GetMinrun(array.Size); // минимальный размер подмассива 
            int currentIndex = 0; // текущий элемент 

            (int StartIndex, int Size)? run; // создаем элемент Run 
            while ((run = NextRun(ref array, ref currentIndex, minrun, comparer)) != null) //заполняем стек Runами 
            {
                runs.Push(((int StartIndex, int Size))run);
                while (runs.Count() >= 3) // если количество Runов больше трех то начинаем мерджить 
                {
                    var x = runs.Pop(); // достаем три элемента стека 
                    var y = runs.Pop();
                    var z = runs.Pop();

                    if (z.Size > x.Size + y.Size && y.Size > x.Size) // если условие выполняется то пушим элементы обратно в стек 
                    {
                        runs.Push(z);
                        runs.Push(y);
                        runs.Push(x);

                        break; // прерываем цикл 
                    }

                    if (z.Size >= x.Size + y.Size) // если условие выполняется 
                    {
                        (int StartIndex, int Size) newRun; //создаем новую переменную для хранения смерженного массива 

                        if (z.Size > x.Size) // если z > x то 
                        {
                            runs.Push(x); // возвращаем х в стек 
                            newRun = Merge(ref array, z.StartIndex, z.Size, y.StartIndex, y.Size, comparer); // а новый подмассив является слиянием z и y
                        }
                        else //иначе 
                        {
                            runs.Push(z); // пушим z 
                            newRun = Merge(ref array, x.StartIndex, x.Size, y.StartIndex, y.Size, comparer); // новый подмассив является слиянием x и у 
                        }


                        runs.Push(newRun); // пушим новый подмассив 

                    }
                    else // иначе 
                    {
                        var newRun = Merge(ref array, y.StartIndex, y.Size, x.StartIndex, x.Size, comparer); // новый подмассив является слиянием у и х 
                        runs.Push(newRun); // пушим новый подмассив 
                        runs.Push(z); // пушим z 
                    }
                }

                while (runs.Count() >= 2) // если Runов больше или равно двум 
                {
                    var x = runs.Pop(); // то достаем два элемента 
                    var y = runs.Pop();

                    var newRun = Merge(ref array, x.StartIndex, x.Size, y.StartIndex, y.Size, comparer); // мерджим их 
                    runs.Push(newRun); // и пушим обратно в стек 
                }
            }
        }

        public static (int StartIndex, int Size) Merge<T>(ref DynamicArray<T> array, int startIndex1, int size1, int startIndex2, int size2, IComparer<T> comparer) // сортировка слиянием с использованием галлопа 
        {
            if (startIndex1 > startIndex2) // startIndex1 должен быть перед startIndex2
            {
                (startIndex1, startIndex2) = (startIndex2, startIndex1);
                (size1, size2) = (size2, size1);
            }

            DynamicArray<T> tempAr = new DynamicArray<T>(size1); // создаем временный массив 
            for (int i = startIndex1; i < size1 + startIndex1; i++) //копируем массив во временный 
            {
                tempAr.Add(array.get(i));
            }
            var current1 = 0; // текущий элемент первого массива 
            var current2 = startIndex2; // текущий элемент второго массива 

            var lastFromLeft = false;
            var count = 0; 

            var until = startIndex2 + size2 - 1;

            for (var currentAr = startIndex1; currentAr <= until; ++currentAr)
            {
                if (current1 == size1)
                {
                    if (lastFromLeft)
                    {
                        lastFromLeft = false;
                        count = 0;
                    }

                    array.set(currentAr,array.get(current2));
                    current2++;
                }
                else if (current2 == startIndex2 + size2)
                {
                    if (!lastFromLeft)
                    {
                        lastFromLeft = true;
                        count = 0;
                    }

                    array.set(currentAr, tempAr.get(current1));
                    current1++;
                }
                else
                {
                    if (comparer.Compare(tempAr.get(current1), array.get(current2)) > 0)
                    {
                        if (lastFromLeft)
                        {
                            lastFromLeft = false;
                            count = 0;
                        }

                        array.set(currentAr, array.get(current2++));
                    }
                    else
                    {
                        if (!lastFromLeft)
                        {
                            lastFromLeft = true;
                            count = 0;
                        }

                        array.set(currentAr, tempAr.get(current1++));
                    }

                }

                count++;

                if (count != GallopSize) continue;

                currentAr++;

                if (lastFromLeft)
                {
                    if (current2 == startIndex2 + size2)
                    {
                        count = 0;
                        currentAr--;
                        continue;
                    }

                    Gallop(ref array, ref tempAr, ref currentAr, ref current1, array.get(current2), comparer); // используем галлоп 
                }
                else
                {
                    if (current1 == size1)
                    {
                        count = 0;
                        currentAr--;
                        continue;
                    }

                    Gallop(ref array, ref array, ref currentAr, ref current2, tempAr.get(current1), comparer); // исспользуем галлоп
                }
                lastFromLeft = !lastFromLeft;
                count = 0;
            }

            return (startIndex1, size1 + size2);
        }
        // функция галлопа 
        private static void Gallop<T>(ref DynamicArray<T> array1, ref DynamicArray<T> array2, ref int currentAr1, ref int currentAr2, T currentOpposite, IComparer<T> comparer) 
        {
            var startIndex = currentAr2; // начальный индекс равен началу второго массива 
            var i = 0;

            while (currentAr2 < array2.Size && comparer.Compare(array2.get(currentAr2), currentOpposite) < 0) // пока текущий элемент второго массива меньше чем конец второго масива и элемент первого массива меньше второго то 
            {
                i++; // прибавляем i 
                currentAr2 += 1 << i; // прибавляем 2^i 
            }

            if (i == 0) 
            {
                currentAr1--;
                return;
            }

            currentAr2 -= 1 << i;
            for(int j = 0; j < currentAr2 - startIndex + 1; j++)
            {
                array1.set(currentAr1 + j, array2.get(startIndex + j));
            }

            currentAr1 += currentAr2 - startIndex;

            currentAr2++;
        }

        public static void insertionSort<T>(ref DynamicArray<T> arr, int left, int right, IComparer<T> comparer) //СОРТИРОВКА ВСТАВКАМИ СЛЕВА НА ПРАВО ИДЕМ
        {
            for (int i = left + 1; i <= right; i++)
            {
                T temp = arr.get(i); // текущий элемент приравниваем элементу массива
                int j = i - 1;
                while (j >= left && comparer.Compare(arr.get(j), temp) > 0) //сравниваем с последующими элементами 
                {
                    arr.set(j + 1, arr.get(j));
                    j--;
                }
                arr.set(j + 1, temp);
            }
        }

        public static void Reverse<T>(ref DynamicArray<T> array, int fromIndex, int toIndex) // "разворачиваем" массив
        {
            var n = (toIndex - fromIndex) / 2; // ищем середину 
            T rever;
            for (var i = 0; i <= n; ++i) // идем к середине меняя все по парам 
            {
                rever = array.get(fromIndex + i);
                array.set(fromIndex + i, array.get(toIndex-i));
                array.set(toIndex - i, rever);
            }
        }

        private static (int StartIndex, int Size)? NextRun<T>(ref DynamicArray<T> array, ref int currentIndex, int minrun, IComparer<T> comparer) //поиск нового Runa
        {
            var diff = array.Size - currentIndex; // считаем оставшееся количество элементов 
            if (diff <= 0) //если массив закончился 
            {
                return null; // возваращем null 
            }
            if (diff == 1) // если остался один элемент 
            {
                return (currentIndex, 1); // возвращаем пару с индексом начала и количеством элементов 
            }

            var startIndex = currentIndex; // начальный индекс приравниваем теккущему индексу 

            var el1 = array.get(currentIndex++); // берем первый элемент и дополнительно увеличиваем индекс 
            var el2 = array.get(currentIndex++); // берем второй элемерт и дополнительно увеличиваем индекс 

            var size = 2; // размер равен двум 

            var invert = comparer.Compare(el1, el2) < 0; // >= 0 значит el1 >= el2

            if (invert) // если invert  = true, то есть el1 < el2 
            {
                for (var prev = el2;
                    currentIndex < array.Size &&
                    comparer.Compare(prev, array.get(currentIndex)) < 0;
                    prev = array.get(currentIndex++), size++)
                { } // прошлый элемент равен второму, пока индекс не дошел до конца массива и текущий элемент больше следующего, то приравниваем элемент следующему
                    // и увеличиваем текущий индекс и размер нового Runa 

                Reverse(ref array, startIndex, currentIndex - 1); // реверсаем массив чтобы расположить все элементы по возрастанию 
            }
            else
            {
                for (var prev = el2;
                    currentIndex < array.Size &&
                    comparer.Compare(prev, array.get(currentIndex)) >= 0;
                    prev = array.get(currentIndex++), size++)
                { }// прошлый элемент равен второму, пока индекс не дошел до конца массива и текущий элемент меньше следующего, то приравниваем элемент следующему
                   // и увеличиваем текущий индекс и размер нового Runa 
            }

            if (size < minrun) // если размер нового Runа меньше минимального размера, то 
            {
                while (currentIndex++ < array.Size && size++ < minrun) { } // увеличиваем текущий индекс и размер Runа до тех пор пока он не станет равным минимальному

                if (currentIndex - 1 == array.Size) // если очередная итерация цикла не выполнилась на первом условии, то увеличиваем размер массива, чтобы полностью закрыть все элементы
                {
                    size++;
                }
                size--; // уменьшаем элемент массива так как инкремент в цикле даст нам лишнюю единицу 
            }

            currentIndex = startIndex + size; // текущий индекс равен начальному плюс размер нового Runa

            insertionSort(ref array, startIndex, startIndex + size - 1, comparer); // сортируем новый найденный Run вставками 

            return (startIndex, size); // возвращаем индекс первого элемента в подмассиве и его размер 
        }

        static void Main(string[] args) {
            Random random = new Random();
            DynamicArray<int> timsort = new DynamicArray<int>();
            int[] systemsort = new int[5000];
            for (int i = 0; i < 5000; i++) // заполняем рандомными числами
            {
                systemsort[i] = random.Next();
                timsort.Add(systemsort[i]);
                
            } 
       
            DoTimSort<int>(ref timsort,Comparer<int>.Create((a, b) => a - b)); // используем тимсорт 
            Array.Sort(systemsort); //используем встроенную функцию 
            int j = 0;
            for (int i = 0; i < timsort.Size; i++) // сравниваем 
            {
                if(systemsort[i] == timsort.get(i))
                {
                    j++;
                }
            }
           if (j == 5000) // проверяем равно ли 
            {
                Console.WriteLine("Good job, Amigo!!"); // вывод 
            }
        }

    }
    
}