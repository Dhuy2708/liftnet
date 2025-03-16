using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils;

public static class QueryUtil
{
    public static IQueryable<T> BuildPaginated<T>(this IQueryable<T> query, QueryCondition conditions)
    {
        var pageSize = conditions.PageSize;
        var pageNumber = conditions.PageNumber;
        return query.AssignPageNumberAndSizeCond(pageNumber, pageSize);
    }

    public static RawQueryParam BuildSqlQuery(string tableName, QueryCondition conditions, List<string>? columnNames = null)
    {
        try
        {
            RawQueryParam result = new();
            StringBuilder sb = new StringBuilder();

            sb.Append("select ");
            AppendSelectColumn(sb, columnNames);

            sb.Append($"from {tableName} ");
            if (!conditions.ConditionItems.Any())
            {
                AppendSortAndPagingCondition(sb, conditions);
                result.Query = sb.ToString();
                result.Params = null;
                return result;
            }

            sb.Append("where ");
            int paramIndex = -1;
            for (int i = 0; i < conditions.ConditionItems.Count; i++)
            {
                paramIndex++;
                var condItem = conditions.ConditionItems[i];
                var op = condItem.Operator;

                // condition item
                if (IsCommonOperator(op))
                {
                    if (condItem.Values.Count() == 1)
                    {
                        AppendQueryConditionItem(sb, condItem, paramIndex);
                        AddParam(result, $"@param{paramIndex}", condItem.Type, condItem.Values[0]);
                    }
                    else if (op == QueryOperator.Equal || op == QueryOperator.NotEqual)
                    {
                        var paramKeys = new List<string>();
                        for (int j = 0; j < condItem.Values.Count(); j++)
                        {
                            paramKeys.Add($"@param{paramIndex + j}");
                        }
                        sb.Append($"{condItem.Property} in({String.Join(", ", paramKeys)}) ");
                        for (int j = 0; j < condItem.Values.Count(); j++)
                        {
                            AddParam(result, paramKeys[j], condItem.Type, condItem.Values[j]);
                        }
                        paramIndex += condItem.Values.Count() - 1;
                    }
                }
                else
                {
                    switch (op)
                    {
                        case QueryOperator.Contains:
                            if (condItem.Type is FilterType.String && condItem.Values.Count() == 1)
                            {
                                sb.Append($"{condItem.Property} like '%{condItem.Values[0]}%' ");
                                AddParam(result, $"@param{paramIndex}", condItem.Type, condItem.Values[0]);
                            }
                            break;
                        case QueryOperator.StartsWith:
                            if (condItem.Type is FilterType.String && condItem.Values.Count() == 1)
                            {
                                sb.Append($"{condItem.Property} like '{condItem.Values[0]}%' ");
                                AddParam(result, $"@param{paramIndex}", condItem.Type, condItem.Values[0]);

                            }
                            break;
                    }
                }

                if (i < conditions.ConditionItems.Count() - 1)
                {
                    AppendLogic(sb, condItem.Logic);
                }
            }
            AppendSortAndPagingCondition(sb, conditions);

            result.Query = sb.ToString();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Error while generating query", ex);
        }
    }

    public static IQueryable<T> AssignPageNumberAndSizeCond<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            throw new ArgumentException("PageNumber and PageSize must be greater than 0.");
        }

        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }


    private static void AppendSelectColumn(StringBuilder sb, List<string>? columnNames)
    {
        if (columnNames != null && columnNames.Any())
        {
            sb.Append(string.Join(",", columnNames));
        }
        else
        {
            sb.Append("* ");
        }
    }

    private static void AppendQueryConditionItem(StringBuilder sb, ConditionItem condItem, int paramIndex) // for common condition
    {
        sb.Append($"{condItem.Property} {GetCommonOperator(condItem.Operator)} @param{paramIndex} ");
    }

    private static void AppendSortAndPagingCondition(StringBuilder sb, QueryCondition cond)
    {
        if (cond.Sort != null && !string.IsNullOrEmpty(cond.Sort.Name))
        {
            sb.Append($"order by {cond.Sort.Name} {GetSortType(cond.Sort.Type)}");
        }

        if (cond.PageNumber > 0 && cond.PageSize > 0)
        {
            int offset = (cond.PageNumber - 1) * cond.PageSize;
            sb.Append($" limit {cond.PageSize} offset {offset}");
        }
    }

    private static void AddParam(RawQueryParam param, string paramKey, FilterType dataType, string value)
    {
        switch (dataType)
        {
            case FilterType.Integer:
                if (int.TryParse(value, out int intValue))
                {
                    param.Params.Add(paramKey, (typeof(int), intValue));
                }
                else
                {
                    throw new ArgumentException($"Invalid integer value for param {paramKey}");
                }
                break;
            case FilterType.String:
                param.Params.Add(paramKey, (typeof(string), value));
                break;

            case FilterType.DateTime:
                if (DateTime.TryParse(value, out DateTime dateValue))
                {
                    param.Params.Add(paramKey, (typeof(DateTime), dateValue));
                }
                else
                {
                    throw new ArgumentException($"Invalid DateTime value for param {paramKey}");
                }
                break;

            case FilterType.Boolean:
                if (bool.TryParse(value, out bool boolValue))
                {
                    param.Params.Add(paramKey, (typeof(bool), boolValue));
                }
                else
                {
                    throw new ArgumentException($"Invalid boolean value for param {paramKey}");
                }
                break;

            default:
                throw new ArgumentException($"Unsupported data type for param {paramKey}");
        }
    }

    private static void AppendLogic(StringBuilder sb, QueryLogic logic)
    {
        sb.Append(GetSqlLogic(logic) + " ");
    }

    private static bool IsCommonOperator(QueryOperator op)
    {
        return !String.IsNullOrEmpty(GetCommonOperator(op));
    }

    private static string GetCommonOperator(QueryOperator op)
    {
        return op switch
        {
            QueryOperator.Equal => "=",
            QueryOperator.NotEqual => "!=",
            QueryOperator.GreaterThan => ">",
            QueryOperator.LessThan => "<",
            QueryOperator.GreaterThanOrEqual => ">=",
            QueryOperator.LessThanOrEqual => "<=",
            _ => String.Empty
        };
    }

    private static string GetSqlLogic(QueryLogic logic)
    {
        return logic switch
        {
            QueryLogic.And => "and",
            QueryLogic.Or => "or",
            QueryLogic.None => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(logic), logic, null)
        };
    }

    private static string GetSortType(SortType type)
    {
        return type switch
        {
            SortType.None => string.Empty,
            SortType.Asc => "asc",
            SortType.Desc => "desc",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
