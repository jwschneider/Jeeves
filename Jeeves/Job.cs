using System;
using System.Collections.Generic;

namespace Jeeves
{
    public class Job
    {
        public int Identity { get; set; }
        public int Value { get; set; }
        public int ReleaseTime { get; set; }
        public int ProcessTime { get; set; }
        public int DueDate { get; set; }
        public int Deadline { get; set; }

        public Job(int identity, int value, int releaseTime, int processTime, int dueDate, int deadline)
        {
            Identity = identity;
            Value = value;
            ReleaseTime = releaseTime;
            ProcessTime = processTime;
            DueDate = dueDate;
            Deadline = deadline;
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
                jobs.Add(new Job(matrix[i, 0], matrix[i, 1], matrix[i, 2], matrix[i, 3], matrix[i, 4], matrix[i, 5]));
            }
            return jobs;
        }
    }
}
