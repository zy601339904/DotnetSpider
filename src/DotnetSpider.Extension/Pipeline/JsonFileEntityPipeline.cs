﻿using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using DotnetSpider.Core;
using System.Linq;
using DotnetSpider.Extension.Model;

namespace DotnetSpider.Extension.Pipeline
{
	/// <summary>
	/// 把解析到的爬虫实体数据序列化成JSON并存到文件中
	/// </summary>
	public class JsonFileEntityPipeline : ModelPipeline
	{
		private readonly Dictionary<string, StreamWriter> _writers = new Dictionary<string, StreamWriter>();

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
			foreach (var writer in _writers)
			{
				writer.Value.Dispose();
			}
		}

		/// <summary>
		/// 把解析到的爬虫实体数据序列化成JSON并存到文件中
		/// </summary>
		/// <param name="model">爬虫实体类的名称</param>
		/// <param name="datas">实体类数据</param>
		/// <param name="spider">爬虫</param>
		/// <returns>最终影响结果数量(如数据库影响行数)</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected override int Process(IModel model, IEnumerable<dynamic> datas, ISpider spider)
		{
			StreamWriter writer;
			var dataFolder = Path.Combine(Env.BaseDirectory, "json", spider.Identity);
			var jsonFile = Path.Combine(dataFolder, $"{model.TableInfo.FullName}.json");
			if (_writers.ContainsKey(jsonFile))
			{
				writer = _writers[jsonFile];
			}
			else
			{
				if (!Directory.Exists(dataFolder))
				{
					Directory.CreateDirectory(dataFolder);
				}
				writer = new StreamWriter(File.OpenWrite(jsonFile), Encoding.UTF8);
				_writers.Add(jsonFile, writer);
			}

			foreach (var entry in datas)
			{
				writer.WriteLine(entry.ToString());
			}
			return datas.Count();
		}
	}
}
