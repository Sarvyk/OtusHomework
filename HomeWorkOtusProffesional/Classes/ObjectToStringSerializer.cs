using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HomeWorkOtusProffesional.Classes
{
    /// <summary>
    /// Класс для сериализации объектов в строку через перечисление полей и свойств
    /// </summary>
    public class ObjectToStringSerializer
    {
        /// <summary>
        /// Сериализует объект в строку, объединяя значения полей и свойств через запятую
        /// </summary>
        /// <param name="obj">Объект для сериализации</param>
        /// <returns>Строка с значениями полей и свойств объекта</returns>
        public string Serialize(object obj)
        {
            if (obj == null)
                return string.Empty;

            var fieldsAndProperties = new List<string>();

            // Получаем все поля и свойства класса
            Type type = obj.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            // Получаем поля
            FieldInfo[] fields = type.GetFields(bindingFlags);
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(obj);
                fieldsAndProperties.Add($"{field.Name}={value ?? "null"}");
            }

            // Получаем свойства
            PropertyInfo[] properties = type.GetProperties(bindingFlags);
            foreach (PropertyInfo property in properties)
            {
                // Пропускаем коллекции
                if (property.GetIndexParameters().Length > 0)
                    continue;

                try
                {
                    object value = property.GetValue(obj);
                    fieldsAndProperties.Add($"{property.Name}={value ?? "null"}");
                }
                catch (Exception ex)
                {
                    fieldsAndProperties.Add($"{property.Name}=Ошибка ({ex.Message})");
                }
            }

            // Объединяем все значения в строку через запятую
            return string.Join(", ", fieldsAndProperties);
        }

        /// <summary>
        /// Десериализует строку в объект
        /// </summary>
        /// <param name="serializedString">Строка с сериализованными данными</param>
        /// <param name="type">Тип объекта для десериализации</param>
        /// <returns>Восстановленный объект</returns>
        public object? Deserialize(string serializedString, Type type)
        {
            if (string.IsNullOrEmpty(serializedString) || type == null)
                return null;

            // Создаем новый экземпляр объекта
            object? obj = Activator.CreateInstance(type);

            // Разбиваем строку на пары "имя=значение"
            string[] pairs = serializedString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pair in pairs)
            {
                int separatorIndex = pair.IndexOf('=');
                if (separatorIndex <= 0)
                    continue;

                string name = pair.Substring(0, separatorIndex).Trim();
                string value = pair.Substring(separatorIndex + 1).Trim();

                // Ищем поле с таким именем
                FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    try
                    {
                        object convertedValue = ConvertValue(value, field.FieldType);
                        field.SetValue(obj, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        // В случае ошибки пропускаем поле
                    }
                }
                else
                {
                    // Ищем свойство с таким именем
                    PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                    if (property != null && property.CanWrite)
                    {
                        try
                        {
                            object convertedValue = ConvertValue(value, property.PropertyType);
                            property.SetValue(obj, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            // В случае ошибки пропускаем свойство
                        }
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Преобразует строковое значение в указанный тип
        /// </summary>
        /// <param name="value">Строковое значение</param>
        /// <param name="targetType">Целевой тип</param>
        /// <returns>Преобразованное значение</returns>
        private object? ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value) || value == "null")
                return null;

            // Обработка специальных случаев
            if (targetType == typeof(string))
                return value;

            if (targetType == typeof(int))
                return int.Parse(value);

            if (targetType == typeof(long))
                return long.Parse(value);

            if (targetType == typeof(double))
                return double.Parse(value);

            if (targetType == typeof(float))
                return float.Parse(value);

            if (targetType == typeof(decimal))
                return decimal.Parse(value);

            if (targetType == typeof(bool))
                return bool.Parse(value);

            if (targetType == typeof(DateTime))
                return DateTime.Parse(value);

            // Для остальных типов пытаемся использовать Convert.ChangeType
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null;
            }
        }
    }
}