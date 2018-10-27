﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Sudoku;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Sudoku
{

  [TestFixture()]
  [Category("Extensions")]
  public class SudokuTest
  {

    private string _easySudokuString = "9.2..54.31...63.255.84.7.6..263.9..1.57.1.29..9.67.53.24.53.6..7.52..3.4.8..4195.";


    [Test()]
    public void ParseSudoku()
    {
      var sudoku = Extensions.Sudoku.Sudoku.Parse(_easySudokuString);

      Assert.AreEqual(sudoku.CellsList[0], 9);
      Assert.AreEqual(sudoku.CellsList[1], 0);
      Assert.AreEqual(sudoku.CellsList[2], 2);
      Assert.AreEqual(sudoku.CellsList[sudoku.CellsList.Count - 2], 5);
      Assert.AreEqual(sudoku.CellsList[sudoku.CellsList.Count - 1], 0);

    }



    [Test()]
    public void Solve_sudoku_with_permutations()
    {
     var sudoku = Extensions.Sudoku.Sudoku.Parse(_easySudokuString);

      //the permutation chromosome should always solve the sudoku in a reasonable time with 700 chromosomes 
      IChromosome chromosome = new SudokuPermutationsChromosome(sudoku);
      var fitness = EvaluatesSudokuChromosome(chromosome, sudoku, 1000, -1);
      Assert.AreEqual(fitness, 0);

    }


    [Test()]
    public void Nearly_solve_sudoku_with_Cells()
    {
      // checking that a simple Sudoku can be tackled using various strategies
      var sudoku = Extensions.Sudoku.Sudoku.Parse(_easySudokuString);

      // Other chromosomes would require more individuals thus more time, so we simply test for significant progresses

      //the cells chromosome should solve the sudoku or nearly in less than 50 generations with 500 chromosomes
      var chromosome = new SudokuCellsChromosome(sudoku);
      var fitness = EvaluatesSudokuChromosome(chromosome, sudoku, 500, 30);
      Assert.Greater(fitness, -20);

    }


    [Test()]
    public void Make_Progresses_with_random_permutations()
    {
      // checking that a simple Sudoku can be tackled using various strategies
      var sudoku = Extensions.Sudoku.Sudoku.Parse(_easySudokuString);


      //the Random permutations chromosome should make significant progresses over 20 generations with 20 individuals

      var chromosome = new SudokuRandomPermutationsChromosome(sudoku, 2, 3);
      var fitness1 = new SudokuFitness(sudoku).Evaluate((ISudokuChromosome) chromosome);
      var fitness2 = EvaluatesSudokuChromosome(chromosome, sudoku, 5, 3);
      Assert.Greater(fitness2, fitness1+20);

    }



    private double EvaluatesSudokuChromosome(IChromosome sudokuChromosome, Extensions.Sudoku.Sudoku sudoku, int populationSize, int generationNb)
    {
      var fitness = new SudokuFitness(sudoku);
      var selection = new EliteSelection();
      var crossover = new UniformCrossover();
      var mutation = new UniformMutation();
      
      var population = new Population(populationSize, populationSize, sudokuChromosome);
      var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
      if (generationNb>-1)
      {
        ga.Termination = new GenerationNumberTermination(generationNb);
      }
      else
      {
        ga.Termination = new FitnessThresholdTermination(0);
      }
      

      ga.Start();

      var bestIndividual = ((ISudokuChromosome)ga.Population.BestChromosome);
      var solutions = bestIndividual.GetSudokus();
      return solutions.Max(solutionSudoku => fitness.Evaluate(solutionSudoku));
    }

  }


  

}
