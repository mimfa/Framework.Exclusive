using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using MiMFa.Engine.Template;
using Aspose.Words.Drawing.Charts;
using System.Text.RegularExpressions;

namespace MiMFa.Engine
{
    [Serializable]
    public class History : History<object>
    {
        public History():base() { }
        public History(General.GenericEventHandler<History<object>, object, bool> undoing, General.GenericEventHandler<History<object>, object, bool> redoing) : base(undoing, redoing) { }
    }

    [Serializable]
    public class History<T>
    {
        public Stack<T> UndoItems = new Stack<T>();
        public Stack<T> RedoItems = new Stack<T>();

        public bool HasUndo
        {
            get => _HasUndo;
            private set
            {
                bool ch = _HasUndo != value;
                _HasUndo = value;
                if(ch) UndoChanged(this, _HasUndo);
            }
        }
        public bool _HasUndo { get; private set; } = false;
        public bool HasRedo { get => _HasRedo;
            private set
            {
                bool ch = _HasRedo != value;
                _HasRedo = value;
                if (ch) RedoChanged(this, _HasRedo);
            }
        }
        public bool _HasRedo { get; private set; } = false;

        public event General.GenericEventHandler<History<T>, T, bool> Undoing =(o,e)=>true;
        public event General.GenericEventHandler<History<T>, T, bool> Redoing = (o, e) => true;
        public event General.GenericEventListener<History<T>, bool> UndoChanged = (o, e) => { };
        public event General.GenericEventListener<History<T>, bool> RedoChanged = (o, e) => { };

        public History() { }
        public History(General.GenericEventHandler<History<T>, T, bool> undoing, General.GenericEventHandler<History<T>, T, bool> redoing):this()
        {
            Undoing = undoing;
            Redoing = redoing;
        }

        public void Clear()
        {
            UndoItems.Clear();
            RedoItems.Clear();
        }
        public T Do<InT>(Func<InT,T> action,InT input) => Done(action(input));
        public T Do(Func<T> action) => Done(action());
        public T Done(T value)
        {
            UndoItems.Push(value);
            HasUndo = UndoItems.Count > 0;
            return value;
        }
        public T Undo()
        {
            try
            {
                if (UndoItems.Count > 0)
                {
                    T p = UndoItems.Pop();
                    if (Undoing(this, p)) RedoItems.Push(p);
                    else UndoItems.Push(p);
                    return p;
                }
            }
            catch { }
            finally
            {
                HasRedo = RedoItems.Count > 0;
                HasUndo = UndoItems.Count > 0;
            }
            return default(T);
        }
        public T Redo()
        {
            try {
                if (RedoItems.Count > 0)
                {
                    T p = RedoItems.Pop();
                    if (Redoing(this, p)) UndoItems.Push(p);
                    else RedoItems.Push(p);
                    return p;
                }
            }
            catch { }
            finally
            {
                HasRedo = RedoItems.Count > 0;
                HasUndo = UndoItems.Count > 0;
            }
            return default(T);
        }
    }
}
