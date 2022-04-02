using System;
using System.Collections.Generic;

namespace Jeeves
{
    public class Job
    {
        public string Identity { get; set; }
        public int Value { get; set; }
        public int ReleaseTime { get; set; }
        public int ProcessTime { get; set; }
        public int DueDate { get; set; }
        public int Deadline { get; set; }

        public Job()
        {

        }

        public Job(string identity, int value, int releaseTime, int processTime, int dueDate, int deadline)
        {
            Identity = identity;
            Value = value;
            ReleaseTime = releaseTime;
            ProcessTime = processTime;
            DueDate = dueDate;
            Deadline = deadline;
        }
        public static Job CreateJobFromArray(int[] array)
        {
            if (array.GetLength(0) != 6)
            {
                throw new ArgumentException($"Rows of input array are of incorrect length: {array.GetLength}");
            }
            return new Job(array[0].ToString(), array[1], array[2], array[3], array[4], array[5]);

        }
        // Array format for job is 6 columns: identity, value, releaseTime, processTime, dueDate, deadline
        public static List<Job> CreateJobsFromMatrix(int[,] matrix)
        {
            if (matrix.GetLength(1) != 6)
            {
                throw new ArgumentException($"Rows of input matrix are of incorrect length {matrix.GetLength(1)}");
            }
            List<Job> jobs = new List<Job>();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                jobs.Add(new Job(matrix[i, 0].ToString(), matrix[i, 1], matrix[i, 2], matrix[i, 3], matrix[i, 4], matrix[i, 5]));
            }
            return jobs;
        }
        public override bool Equals(object obj)
        {
            if (obj is Job)
            {
                return String.Equals(((Job)obj).Identity, Identity)
                    && DateTime.Equals(((Job)obj).ReleaseTime, ReleaseTime)
                    && TimeSpan.Equals(((Job)obj).ProcessTime, ProcessTime)
                    && DateTime.Equals(((Job)obj).DueDate, DueDate)
                    && DateTime.Equals(((Job)obj).Deadline, Deadline)
                    && ((Job)obj).Value == Value;
            }
            else return false;
        }
        public override string ToString()
        {
            return $"{{Identity:{Identity}, ReleaseTime{ReleaseTime}, ProcessTime{ProcessTime}, DueDate{DueDate}, Deadline{Deadline}, Value{Value}}}";
        }
    }
}
