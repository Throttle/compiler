using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCore
{
	public class ElementCollection: System.Collections.CollectionBase
	{
		public ElementCollection()
		{
			// empty
		}

		public ElementCollection(Element value)
		{
			Add(value);
		}

		public virtual void Add(Element value)
		{
			this.List.Add(value);
		}

		public virtual Element this[int index]
		{
			get
			{
				return (Element) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(ElementCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Element Current
			{
				get
				{
					return (Element) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Element) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}
        
		public new virtual ElementCollection.Enumerator GetEnumerator()
		{
			return new ElementCollection.Enumerator(this);
		}
	}

	public class FunctionCollection: System.Collections.CollectionBase
	{
		public FunctionCollection()
		{
			// empty
		}

		public FunctionCollection(Function value)
		{
			Add(value);
		}

		public virtual void Add(Function value)
		{
			this.List.Add(value);
		}

		public virtual Function this[int index]
		{
			get
			{
				return (Function) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(FunctionCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Function Current
			{
				get
				{
					return (Function) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Function) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}
        
		public new virtual FunctionCollection.Enumerator GetEnumerator()
		{
			return new FunctionCollection.Enumerator(this);
		}
	}

	public class VariableCollection: System.Collections.CollectionBase
	{
		public VariableCollection()
		{
			// empty
		}

		public VariableCollection(Variable value)
		{
			Add(value);
		}

		public virtual void Add(Variable value)
		{
			this.List.Add(value);
		}

		public virtual Variable this[int index]
		{
			get
			{
				return (Variable) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(VariableCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Variable Current
			{
				get
				{
					return (Variable) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Variable) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}
        
		public new virtual VariableCollection.Enumerator GetEnumerator()
		{
			return new VariableCollection.Enumerator(this);
		}
	}

	public class StructureCollection: System.Collections.CollectionBase
{
	public StructureCollection()
	{
		// empty
	}

	public StructureCollection(Structure value)
	{
		Add(value);
	}

	public virtual void Add(Structure value)
	{
		this.List.Add(value);
	}
	
	public virtual Structure this[int index]
	{
		get
		{
			return (Structure) this.List[index];
		}
		set
		{
			this.List[index] = value;
		}
	}

	public class Enumerator: System.Collections.IEnumerator
	{
		private System.Collections.IEnumerator wrapped;

		public Enumerator(StructureCollection collection)
		{
			this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
		}

		public Structure Current
		{
			get
			{
				return (Structure) (this.wrapped.Current);
			}
		}

		object System.Collections.IEnumerator.Current
		{
			get
			{
				return (Structure) (this.wrapped.Current);
			}
		}

		public bool MoveNext()
		{
			return this.wrapped.MoveNext();
		}

		public void Reset()
		{
			this.wrapped.Reset();
		}
	}
        
	public new virtual StructureCollection.Enumerator GetEnumerator()
	{
		return new StructureCollection.Enumerator(this);
	}
}

	public class ParameterCollection: System.Collections.CollectionBase
	{
		public ParameterCollection()
		{
			// empty
		}

		public ParameterCollection(Parameter value)
		{
			Add(value);
		}

		public virtual void Add(Parameter value)
		{
			this.List.Add(value);
		}

		public virtual Parameter this[int index]
		{
			get
			{
				return (Parameter) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(ParameterCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Parameter Current
			{
				get
				{
					return (Parameter) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Parameter) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}
        
		public new virtual ParameterCollection.Enumerator GetEnumerator()
		{
			return new ParameterCollection.Enumerator(this);
		}
	}

	public class StatementCollection: System.Collections.CollectionBase
	{
		public StatementCollection()
		{
			// empty
		}

		public StatementCollection(Statement value)
		{
			Add(value);
		}

		public virtual void Add(Statement value)
		{
			this.List.Add(value);
		}

		public virtual Statement this[int index]
		{
			get
			{
				return (Statement) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(StatementCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Statement Current
			{
				get
				{
					return (Statement) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Statement) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}
        
		public new virtual StatementCollection.Enumerator GetEnumerator()
		{
			return new StatementCollection.Enumerator(this);
		}
	}

	public class ArgumentCollection: System.Collections.CollectionBase
	{
		public ArgumentCollection()
		{
			// empty
		}

		public ArgumentCollection(Argument value)
		{
			Add(value);
		}

		public virtual void Add(Argument value)
		{
			this.List.Add(value);
		}

		public virtual Argument this[int index]
		{
			get
			{
				return (Argument) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(ArgumentCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Argument Current
			{
				get
				{
					return (Argument) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Argument) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}
        
		public new virtual ArgumentCollection.Enumerator GetEnumerator()
		{
			return new ArgumentCollection.Enumerator(this);
		}
	}
}
