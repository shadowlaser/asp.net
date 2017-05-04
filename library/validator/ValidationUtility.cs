using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Seagull2.StraPosit.WebApi.Utility
{
    /// <summary>
    /// 给object尾部添加验证器
    /// Name：有验证器的属性名
    /// Validators：一个属性的所有验证器
    /// </summary>
    public class ValidationNode
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<ValidationAttribute> Validators { get; set; } = new List<ValidationAttribute>();
    }

    public class ValidatorHandler
    {
        /// <summary>
        /// 创建验证器节点
        /// </summary>
        /// <typeparam name="T">需要获取验证器的类型</typeparam>
        /// <returns>返回ValidationNode</returns>
        public List<ValidationNode> CreateValidators<T>()
        {
            List<ValidationNode> nodes = new List<ValidationNode>();
            Type t = typeof(T);

            foreach (PropertyInfo p in t.GetProperties())
            {
                ValidationNode node = new ValidationNode();

                node.Name = p.Name;
                node.Type = p.PropertyType.FullName;
                object[] o = p.GetCustomAttributes(true);

                foreach (var i in o)
                {
                    Type baseType = i.GetType().BaseType;
                    if (baseType == typeof(ValidationAttribute))
                    {
                        node.Validators.Add((ValidationAttribute)i);
                    }
                    else if (i.GetType() == typeof(DescriptionAttribute))
                    {
                        node.Key = ((DescriptionAttribute)i).Description;
                    }
                }
                if (node.Validators.Count > 0)
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }
    }
}