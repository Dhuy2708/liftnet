using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiftNet.Utility.Utils
{
    public static class QueryIndexUtil
    {
        public static RawQueryParam BuildIndexQuery(QueryCondition conditions, List<string>? columnNames = null)
        {
            try
            {
                RawQueryParam result = new();
                StringBuilder sb = new StringBuilder();

                sb.Append("select ");
                AppendSelectColumn(sb, columnNames);

                sb.Append("from c ");
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
                    var condItem = conditions.ConditionItems[i];
                    var op = condItem.Operator;

                    // Add logic operator before condition (except for first item)
                    if (i > 0)
                    {
                        AppendLogic(sb, condItem.Logic);
                    }

                    paramIndex++;
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
                            sb.Append($"c.{condItem.Property} in({String.Join(", ", paramKeys)}) ");
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
                                    sb.Append($"CONTAINS(c.{condItem.Property}, @param{paramIndex}) ");
                                    AddParam(result, $"@param{paramIndex}", condItem.Type, condItem.Values[0]);
                                }
                                break;
                            case QueryOperator.StartsWith:
                                if (condItem.Type is FilterType.String && condItem.Values.Count() == 1)
                                {
                                    sb.Append($"STARTSWITH(c.{condItem.Property}, @param{paramIndex}) ");
                                    AddParam(result, $"@param{paramIndex}", condItem.Type, condItem.Values[0]);
                                }
                                break;
                        }
                    }
                }
                AppendSortAndPagingCondition(sb, conditions);

                result.Query = sb.ToString();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while generating index query", ex);
            }
        }

        private static void AppendSelectColumn(StringBuilder sb, List<string>? columnNames)
        {
            if (columnNames != null && columnNames.Any())
            {
                sb.Append(string.Join(",", columnNames.Select(col => $"c.{col}")));
                sb.Append(" ");
            }
            else
            {
                sb.Append("* ");
            }
        }

        private static void AppendQueryConditionItem(StringBuilder sb, ConditionItem condItem, int paramIndex)
        {
            sb.Append($"c.{condItem.Property} {GetCommonOperator(condItem.Operator)} @param{paramIndex} ");
        }

        private static void AppendSortAndPagingCondition(StringBuilder sb, QueryCondition cond)
        {
            if (cond.Sort != null && !string.IsNullOrEmpty(cond.Sort.Name))
            {
                sb.Append($"order by c.{cond.Sort.Name} {GetSortType(cond.Sort.Type)} ");
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
            var logicStr = GetSqlLogic(logic);
            if (!string.IsNullOrEmpty(logicStr))
            {
                sb.Append($"{logicStr} ");
            }
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
} 