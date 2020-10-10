using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable] public class RoundData
{
    [SerializeField] public List<ProblemData> problems;
    [SerializeField] public DateTime dateTime;

    [SerializeField] public float totalTime;
    [SerializeField] public float averageTime;
    [SerializeField] public int correctProblemCount;

    public RoundData()
    {
        dateTime = DateTime.Now;
        problems = new List<ProblemData>();
    }

    public void CalculateRoundInfo()
    {
        IEnumerable<ProblemData> correctProblems =
            from problem in problems
            where problem.Passed
            select problem;
        correctProblemCount = correctProblems.Count<ProblemData>();

        totalTime = 0;
        foreach (ProblemData problem in problems)
        {
            totalTime += problem.ExecutionTime;
        }

        averageTime = totalTime / correctProblemCount;
    }
}