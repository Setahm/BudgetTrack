using BudgetTrack.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace BudgetTrack.Services
{
    public class ExpensePredictionInput
    {
        public float Amount { get; set; }
        public float Month { get; set; }
        public float Year { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }

    public class ExpensePredictionOutput
    {
        [ColumnName("Score")]
        public float PredictedAmount { get; set; }
    }

    public class PredictionService
    {
        private readonly BudgetTrackDbContext _context;

        public PredictionService(BudgetTrackDbContext context)
        {
            _context = context;
        }

        public Dictionary<int, float> PredictNextYearBudgetPerDepartment(int companyId)
        {
            var departments = _context.Departments
                .Where(d => d.CompanyId == companyId)
                .ToList();

            var result = new Dictionary<int, float>();
            int nextYear = DateTime.Now.Year + 1;

            foreach (var dept in departments)
            {
                var expenses = _context.Expenses
                    .Where(e => e.DepartmentId == dept.Id)
                    .OrderBy(e => e.ExpenseDate)
                    .ToList();

                if (!expenses.Any())
                {
                    float fallback = Convert.ToSingle(dept.AnnualBudget);
                    result[dept.Id] = fallback;

                    bool exists = _context.Predictions
                        .Any(p => p.DepartmentId == dept.Id && p.Year == nextYear);

                    if (!exists)
                    {
                        var prediction = new Prediction
                        {
                            DepartmentId = dept.Id,
                            DepartmentName = dept.Name,
                            Year = nextYear,
                            PredictedAmount = (decimal)fallback,
                            CreatedAt = DateTime.Now
                        };

                        _context.Predictions.Add(prediction);
                    }
                }

                float avg = Convert.ToSingle(expenses.Average(e => e.Amount));
                float total = Convert.ToSingle(expenses.Sum(e => e.Amount));

                float growth = 0;
                if (expenses.Count > 1)
                {
                    float first = Convert.ToSingle(expenses.First().Amount);
                    float last = Convert.ToSingle(expenses.Last().Amount);

                    if (first > 0)
                        growth = (last - first) / first;
                }

                float statisticalPrediction = (avg * 12) * (1 + growth);

                var mlContext = new MLContext();

                var trainingData = expenses.Select(e => new ExpensePredictionInput
                {
                    Amount = Convert.ToSingle(e.Amount),
                    Month = e.ExpenseDate.Month,
                    Year = e.ExpenseDate.Year,
                    Category = e.Category,
                    DepartmentId = dept.Id
                });

                var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

                var pipeline = mlContext.Transforms.CopyColumns("Label", "Amount")
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding("CategoryEncoded", "Category"))
                    .Append(mlContext.Transforms.Concatenate("Features", "Month", "Year", "CategoryEncoded"))
                    .Append(mlContext.Regression.Trainers.FastTree());

                var model = pipeline.Fit(dataView);

                var engine = mlContext.Model.CreatePredictionEngine<ExpensePredictionInput, ExpensePredictionOutput>(model);

                float mlTotal = 0;

                for (int month = 1; month <= 12; month++)
                {
                    var sample = new ExpensePredictionInput
                    {
                        Amount = 0,
                        Month = month,
                        Year = nextYear,
                        Category = expenses
                            .GroupBy(e => e.Category)
                            .OrderByDescending(g => g.Count())
                            .First().Key,
                        DepartmentId = dept.Id
                    };

                    mlTotal += engine.Predict(sample).PredictedAmount;
                }

                float finalPrediction =
                    (mlTotal * 0.6f) +
                    (statisticalPrediction * 0.4f);

                result[dept.Id] = finalPrediction;

                bool exists2 = _context.Predictions
                    .Any(p => p.DepartmentId == dept.Id && p.Year == nextYear);

                if (!exists2)
                {
                    var predictionRecord = new Prediction
                    {
                        DepartmentId = dept.Id,
                        DepartmentName = dept.Name,
                        Year = nextYear,
                        PredictedAmount = (decimal)finalPrediction,
                        CreatedAt = DateTime.Now
                    };

                    _context.Predictions.Add(predictionRecord);
                }
            }

            _context.SaveChanges();

            return result;
        }
    }
}