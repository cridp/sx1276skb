using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SemtechLib.Controls.HexBoxCtrl
{
	[ToolboxBitmap(typeof(HexBox), "HexBox.bmp")]
	public class HexBox : Control
	{
		private interface IKeyInterpreter
		{
			void Activate();

			void Deactivate();

			bool PreProcessWmKeyUp(ref Message m);

			bool PreProcessWmChar(ref Message m);

			bool PreProcessWmKeyDown(ref Message m);

			PointF GetCaretPointF(long byteIndex);
		}

		private sealed class EmptyKeyInterpreter : IKeyInterpreter
		{
			private HexBox _hexBox;

			public EmptyKeyInterpreter(HexBox hexBox)
			{
				_hexBox = hexBox;
			}

			public void Activate()
			{
			}

			public void Deactivate()
			{
			}

			public bool PreProcessWmKeyUp(ref Message m)
			{
				return _hexBox.BasePreProcessMessage(ref m);
			}

			public bool PreProcessWmChar(ref Message m)
			{
				return _hexBox.BasePreProcessMessage(ref m);
			}

			public bool PreProcessWmKeyDown(ref Message m)
			{
				return _hexBox.BasePreProcessMessage(ref m);
			}

			public PointF GetCaretPointF(long byteIndex)
			{
				return default(PointF);
			}
		}

		private class KeyInterpreter : IKeyInterpreter
		{
			private delegate bool MessageDelegate(ref Message m);

			protected HexBox _hexBox;

			protected bool _shiftDown;

			private bool _mouseDown;

			private BytePositionInfo _bpiStart;

			private BytePositionInfo _bpi;

			private Dictionary<Keys, MessageDelegate> _messageHandlers;

			private Dictionary<Keys, MessageDelegate> MessageHandlers
			{
				get
				{
					if (_messageHandlers == null)
					{
						_messageHandlers = new Dictionary<Keys, MessageDelegate>();
						_messageHandlers.Add(Keys.Left, PreProcessWmKeyDown_Left);
						_messageHandlers.Add(Keys.Up, PreProcessWmKeyDown_Up);
						_messageHandlers.Add(Keys.Right, PreProcessWmKeyDown_Right);
						_messageHandlers.Add(Keys.Down, PreProcessWmKeyDown_Down);
						_messageHandlers.Add(Keys.Prior, PreProcessWmKeyDown_PageUp);
						_messageHandlers.Add(Keys.Next | Keys.Shift, PreProcessWmKeyDown_PageDown);
						_messageHandlers.Add(Keys.Left | Keys.Shift, PreProcessWmKeyDown_ShiftLeft);
						_messageHandlers.Add(Keys.Up | Keys.Shift, PreProcessWmKeyDown_ShiftUp);
						_messageHandlers.Add(Keys.Right | Keys.Shift, PreProcessWmKeyDown_ShiftRight);
						_messageHandlers.Add(Keys.Down | Keys.Shift, PreProcessWmKeyDown_ShiftDown);
						_messageHandlers.Add(Keys.Tab, PreProcessWmKeyDown_Tab);
						_messageHandlers.Add(Keys.Back, PreProcessWmKeyDown_Back);
						_messageHandlers.Add(Keys.Delete, PreProcessWmKeyDown_Delete);
						_messageHandlers.Add(Keys.Home, PreProcessWmKeyDown_Home);
						_messageHandlers.Add(Keys.End, PreProcessWmKeyDown_End);
						_messageHandlers.Add(Keys.ShiftKey | Keys.Shift, PreProcessWmKeyDown_ShiftShiftKey);
						_messageHandlers.Add(Keys.C | Keys.Control, PreProcessWmKeyDown_ControlC);
						_messageHandlers.Add(Keys.X | Keys.Control, PreProcessWmKeyDown_ControlX);
						_messageHandlers.Add(Keys.V | Keys.Control, PreProcessWmKeyDown_ControlV);
					}
					return _messageHandlers;
				}
			}

			public KeyInterpreter(HexBox hexBox)
			{
				_hexBox = hexBox;
			}

			public virtual void Activate()
			{
				_hexBox.MouseDown += BeginMouseSelection;
				_hexBox.MouseMove += UpdateMouseSelection;
				_hexBox.MouseUp += EndMouseSelection;
			}

			public virtual void Deactivate()
			{
				_hexBox.MouseDown -= BeginMouseSelection;
				_hexBox.MouseMove -= UpdateMouseSelection;
				_hexBox.MouseUp -= EndMouseSelection;
			}

			private void BeginMouseSelection(object sender, MouseEventArgs e)
			{
				if (e.Button == MouseButtons.Left)
				{
					_mouseDown = true;
					if (!_shiftDown)
					{
						_bpiStart = new BytePositionInfo(_hexBox._bytePos, _hexBox._byteCharacterPos);
						_hexBox.ReleaseSelection();
					}
					else
					{
						UpdateMouseSelection(this, e);
					}
				}
			}

			private void UpdateMouseSelection(object sender, MouseEventArgs e)
			{
				if (!_mouseDown)
				{
					return;
				}
				_bpi = GetBytePositionInfo(new Point(e.X, e.Y));
				var index = _bpi.Index;
				long num;
				long num2;
				if (index < _bpiStart.Index)
				{
					num = index;
					num2 = _bpiStart.Index - index;
				}
				else if (index > _bpiStart.Index)
				{
					num = _bpiStart.Index;
					num2 = index - num;
				}
				else
				{
					num = _hexBox._bytePos;
					num2 = 0L;
				}
				if (num != _hexBox._bytePos || num2 != _hexBox._selectionLength)
				{
					_hexBox.InternalSelect(num, num2);
					_hexBox.ScrollByteIntoView(_bpi.Index);
				}
				var bytePos = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				if (_bpiStart.Index <= bytePos)
				{
					selectionLength += _hexBox.HorizontalByteCount;
					_hexBox.ScrollByteIntoView(bytePos + selectionLength);
					return;
				}
				selectionLength -= _hexBox.HorizontalByteCount;
				if (selectionLength < 0)
				{
					bytePos = _bpiStart.Index;
					selectionLength = -selectionLength;
				}
				else
				{
					bytePos += _hexBox.HorizontalByteCount;
					selectionLength -= _hexBox.HorizontalByteCount;
				}
				_hexBox.ScrollByteIntoView();
			}

			private void EndMouseSelection(object sender, MouseEventArgs e)
			{
				_mouseDown = false;
			}

			public virtual bool PreProcessWmKeyDown(ref Message m)
			{
				var keys = (Keys)m.WParam.ToInt32();
				var keys2 = keys | ModifierKeys;
				var flag = MessageHandlers.ContainsKey(keys2);
				if (flag && RaiseKeyDown(keys2))
				{
					return true;
				}
				MessageDelegate messageDelegate = (flag ? MessageHandlers[keys2] : (messageDelegate = PreProcessWmKeyDown_Default));
				return messageDelegate(ref m);
			}

			protected bool PreProcessWmKeyDown_Default(ref Message m)
			{
				_hexBox.ScrollByteIntoView();
				return _hexBox.BasePreProcessMessage(ref m);
			}

			protected bool RaiseKeyDown(Keys keyData)
			{
				var keyEventArgs = new KeyEventArgs(keyData);
				_hexBox.OnKeyDown(keyEventArgs);
				return keyEventArgs.Handled;
			}

			protected virtual bool PreProcessWmKeyDown_Left(ref Message m)
			{
				return PerformPosMoveLeft();
			}

			protected virtual bool PreProcessWmKeyDown_Up(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				if (bytePos != 0 || byteCharacterPos != 0)
				{
					bytePos = Math.Max(-1L, bytePos - _hexBox.HorizontalByteCount);
					if (bytePos == -1)
					{
						return true;
					}
					_hexBox.SetPosition(bytePos);
					if (bytePos < _hexBox._startByte)
					{
						_hexBox.PerformScrollLineUp();
					}
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
				}
				_hexBox.ScrollByteIntoView();
				_hexBox.ReleaseSelection();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Right(ref Message m)
			{
				return PerformPosMoveRight();
			}

			protected virtual bool PreProcessWmKeyDown_Down(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var num = _hexBox._byteCharacterPos;
				if (bytePos == _hexBox._byteProvider.Length && num == 0)
				{
					return true;
				}
				bytePos = Math.Min(_hexBox._byteProvider.Length, bytePos + _hexBox.HorizontalByteCount);
				if (bytePos == _hexBox._byteProvider.Length)
				{
					num = 0;
				}
				_hexBox.SetPosition(bytePos, num);
				if (bytePos > _hexBox._endByte - 1)
				{
					_hexBox.PerformScrollLineDown();
				}
				_hexBox.UpdateCaret();
				_hexBox.ScrollByteIntoView();
				_hexBox.ReleaseSelection();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_PageUp(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				if (bytePos == 0 && byteCharacterPos == 0)
				{
					return true;
				}
				bytePos = Math.Max(0L, bytePos - _hexBox._iHexMaxBytes);
				if (bytePos == 0)
				{
					return true;
				}
				_hexBox.SetPosition(bytePos);
				if (bytePos < _hexBox._startByte)
				{
					_hexBox.PerformScrollPageUp();
				}
				_hexBox.ReleaseSelection();
				_hexBox.UpdateCaret();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_PageDown(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var num = _hexBox._byteCharacterPos;
				if (bytePos == _hexBox._byteProvider.Length && num == 0)
				{
					return true;
				}
				bytePos = Math.Min(_hexBox._byteProvider.Length, bytePos + _hexBox._iHexMaxBytes);
				if (bytePos == _hexBox._byteProvider.Length)
				{
					num = 0;
				}
				_hexBox.SetPosition(bytePos, num);
				if (bytePos > _hexBox._endByte - 1)
				{
					_hexBox.PerformScrollPageDown();
				}
				_hexBox.ReleaseSelection();
				_hexBox.UpdateCaret();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftLeft(ref Message m)
			{
				var num = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				if (num + selectionLength < 1)
				{
					return true;
				}
				if (num + selectionLength <= _bpiStart.Index)
				{
					if (num == 0)
					{
						return true;
					}
					num--;
					selectionLength++;
				}
				else
				{
					selectionLength = Math.Max(0L, selectionLength - 1);
				}
				_hexBox.ScrollByteIntoView();
				_hexBox.InternalSelect(num, selectionLength);
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftUp(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				if (bytePos - _hexBox.HorizontalByteCount < 0 && bytePos <= _bpiStart.Index)
				{
					return true;
				}
				if (_bpiStart.Index >= bytePos + selectionLength)
				{
					bytePos -= _hexBox.HorizontalByteCount;
					selectionLength += _hexBox.HorizontalByteCount;
					_hexBox.InternalSelect(bytePos, selectionLength);
					_hexBox.ScrollByteIntoView();
				}
				else
				{
					selectionLength -= _hexBox.HorizontalByteCount;
					if (selectionLength < 0)
					{
						bytePos = _bpiStart.Index + selectionLength;
						selectionLength = -selectionLength;
						_hexBox.InternalSelect(bytePos, selectionLength);
						_hexBox.ScrollByteIntoView();
					}
					else
					{
						selectionLength -= _hexBox.HorizontalByteCount;
						_hexBox.InternalSelect(bytePos, selectionLength);
						_hexBox.ScrollByteIntoView(bytePos + selectionLength);
					}
				}
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftRight(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				if (bytePos + selectionLength >= _hexBox._byteProvider.Length)
				{
					return true;
				}
				if (_bpiStart.Index <= bytePos)
				{
					selectionLength++;
					_hexBox.InternalSelect(bytePos, selectionLength);
					_hexBox.ScrollByteIntoView(bytePos + selectionLength);
				}
				else
				{
					bytePos++;
					selectionLength = Math.Max(0L, selectionLength - 1);
					_hexBox.InternalSelect(bytePos, selectionLength);
					_hexBox.ScrollByteIntoView();
				}
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftDown(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				var length = _hexBox._byteProvider.Length;
				if (bytePos + selectionLength + _hexBox.HorizontalByteCount > length)
				{
					return true;
				}
				if (_bpiStart.Index <= bytePos)
				{
					selectionLength += _hexBox.HorizontalByteCount;
					_hexBox.InternalSelect(bytePos, selectionLength);
					_hexBox.ScrollByteIntoView(bytePos + selectionLength);
				}
				else
				{
					selectionLength -= _hexBox.HorizontalByteCount;
					if (selectionLength < 0)
					{
						bytePos = _bpiStart.Index;
						selectionLength = -selectionLength;
					}
					else
					{
						bytePos += _hexBox.HorizontalByteCount;
					}
					_hexBox.InternalSelect(bytePos, selectionLength);
					_hexBox.ScrollByteIntoView();
				}
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Tab(ref Message m)
			{
				if (_hexBox._stringViewVisible && _hexBox._keyInterpreter.GetType() == typeof(KeyInterpreter))
				{
					_hexBox.ActivateStringKeyInterpreter();
					_hexBox.ScrollByteIntoView();
					_hexBox.ReleaseSelection();
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
					return true;
				}
				if (_hexBox.Parent == null)
				{
					return true;
				}
				_hexBox.Parent.SelectNextControl(_hexBox, forward: true, tabStopOnly: true, nested: true, wrap: true);
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftTab(ref Message m)
			{
				if (_hexBox._keyInterpreter is StringKeyInterpreter)
				{
					_shiftDown = false;
					_hexBox.ActivateKeyInterpreter();
					_hexBox.ScrollByteIntoView();
					_hexBox.ReleaseSelection();
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
					return true;
				}
				if (_hexBox.Parent == null)
				{
					return true;
				}
				_hexBox.Parent.SelectNextControl(_hexBox, forward: false, tabStopOnly: true, nested: true, wrap: true);
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Back(ref Message m)
			{
				if (!_hexBox._byteProvider.SupportsDeleteBytes())
				{
					return true;
				}
				if (_hexBox.ReadOnly)
				{
					return true;
				}
				var bytePos = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				var num = ((_hexBox._byteCharacterPos == 0 && selectionLength == 0) ? (bytePos - 1) : bytePos);
				if (num < 0 && selectionLength < 1)
				{
					return true;
				}
				var length = ((selectionLength > 0) ? selectionLength : 1);
				_hexBox._byteProvider.DeleteBytes(Math.Max(0L, num), length);
				_hexBox.UpdateScrollSize();
				if (selectionLength == 0)
				{
					PerformPosMoveLeftByte();
				}
				_hexBox.ReleaseSelection();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Delete(ref Message m)
			{
				if (!_hexBox._byteProvider.SupportsDeleteBytes())
				{
					return true;
				}
				if (_hexBox.ReadOnly)
				{
					return true;
				}
				var bytePos = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				if (bytePos >= _hexBox._byteProvider.Length)
				{
					return true;
				}
				var length = ((selectionLength > 0) ? selectionLength : 1);
				_hexBox._byteProvider.DeleteBytes(bytePos, length);
				_hexBox.UpdateScrollSize();
				_hexBox.ReleaseSelection();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Home(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				if (bytePos < 1)
				{
					return true;
				}
				bytePos = 0L;
				byteCharacterPos = 0;
				_hexBox.SetPosition(bytePos, byteCharacterPos);
				_hexBox.ScrollByteIntoView();
				_hexBox.UpdateCaret();
				_hexBox.ReleaseSelection();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_End(ref Message m)
			{
				var bytePos = _hexBox._bytePos;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				if (bytePos >= _hexBox._byteProvider.Length - 1)
				{
					return true;
				}
				bytePos = _hexBox._byteProvider.Length;
				byteCharacterPos = 0;
				_hexBox.SetPosition(bytePos, byteCharacterPos);
				_hexBox.ScrollByteIntoView();
				_hexBox.UpdateCaret();
				_hexBox.ReleaseSelection();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftShiftKey(ref Message m)
			{
				if (_mouseDown)
				{
					return true;
				}
				if (_shiftDown)
				{
					return true;
				}
				_shiftDown = true;
				if (_hexBox._selectionLength > 0)
				{
					return true;
				}
				_bpiStart = new BytePositionInfo(_hexBox._bytePos, _hexBox._byteCharacterPos);
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ControlC(ref Message m)
			{
				_hexBox.Copy();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ControlX(ref Message m)
			{
				_hexBox.Cut();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ControlV(ref Message m)
			{
				_hexBox.Paste();
				return true;
			}

			public virtual bool PreProcessWmChar(ref Message m)
			{
				if (ModifierKeys == Keys.Control)
				{
					return _hexBox.BasePreProcessMessage(ref m);
				}
				var flag = _hexBox._byteProvider.SupportsWriteByte();
				var flag2 = _hexBox._byteProvider.SupportsInsertBytes();
				var flag3 = _hexBox._byteProvider.SupportsDeleteBytes();
				var bytePos = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				var num = _hexBox._byteCharacterPos;
				if ((!flag && bytePos != _hexBox._byteProvider.Length) || (!flag2 && bytePos == _hexBox._byteProvider.Length))
				{
					return _hexBox.BasePreProcessMessage(ref m);
				}
				var c = (char)m.WParam.ToInt32();
				if (Uri.IsHexDigit(c))
				{
					if (RaiseKeyPress(c))
					{
						return true;
					}
					if (_hexBox.ReadOnly)
					{
						return true;
					}
					var flag4 = bytePos == _hexBox._byteProvider.Length;
					if (!flag4 && flag2 && _hexBox.InsertActive && num == 0)
					{
						flag4 = true;
					}
					if (flag3 && flag2 && selectionLength > 0)
					{
						_hexBox._byteProvider.DeleteBytes(bytePos, selectionLength);
						flag4 = true;
						num = 0;
						_hexBox.SetPosition(bytePos, num);
					}
					_hexBox.ReleaseSelection();
					var text = ((byte)((!flag4) ? _hexBox._byteProvider.ReadByte(bytePos) : 0)).ToString("X", Thread.CurrentThread.CurrentCulture);
					if (text.Length == 1)
					{
						text = "0" + text;
					}
					var text2 = c.ToString();
					text2 = ((num != 0) ? (text.Substring(0, 1) + text2) : (text2 + text.Substring(1, 1)));
					var b = byte.Parse(text2, NumberStyles.AllowHexSpecifier, Thread.CurrentThread.CurrentCulture);
					if (flag4)
					{
						_hexBox._byteProvider.InsertBytes(bytePos, new byte[1] { b });
					}
					else
					{
						_hexBox._byteProvider.WriteByte(bytePos, b);
					}
					PerformPosMoveRight();
					_hexBox.Invalidate();
					return true;
				}
				return _hexBox.BasePreProcessMessage(ref m);
			}

			protected bool RaiseKeyPress(char keyChar)
			{
				var keyPressEventArgs = new KeyPressEventArgs(keyChar);
				_hexBox.OnKeyPress(keyPressEventArgs);
				return keyPressEventArgs.Handled;
			}

			public virtual bool PreProcessWmKeyUp(ref Message m)
			{
				var keys = (Keys)m.WParam.ToInt32();
				var keys2 = keys | ModifierKeys;
				var keys3 = keys2;
				if ((keys3 == Keys.ShiftKey || keys3 == Keys.Insert) && RaiseKeyUp(keys2))
				{
					return true;
				}
				switch (keys2)
				{
				case Keys.ShiftKey:
					_shiftDown = false;
					return true;
				case Keys.Insert:
					return PreProcessWmKeyUp_Insert(ref m);
				default:
					return _hexBox.BasePreProcessMessage(ref m);
				}
			}

			protected virtual bool PreProcessWmKeyUp_Insert(ref Message m)
			{
				_hexBox.InsertActive = !_hexBox.InsertActive;
				return true;
			}

			protected bool RaiseKeyUp(Keys keyData)
			{
				var keyEventArgs = new KeyEventArgs(keyData);
				_hexBox.OnKeyUp(keyEventArgs);
				return keyEventArgs.Handled;
			}

			protected virtual bool PerformPosMoveLeft()
			{
				var num = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				if (selectionLength != 0)
				{
					byteCharacterPos = 0;
					_hexBox.SetPosition(num, byteCharacterPos);
					_hexBox.ReleaseSelection();
				}
				else
				{
					if (num == 0 && byteCharacterPos == 0)
					{
						return true;
					}
					if (byteCharacterPos > 0)
					{
						byteCharacterPos--;
					}
					else
					{
						num = Math.Max(0L, num - 1);
						byteCharacterPos++;
					}
					_hexBox.SetPosition(num, byteCharacterPos);
					if (num < _hexBox._startByte)
					{
						_hexBox.PerformScrollLineUp();
					}
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
				}
				_hexBox.ScrollByteIntoView();
				return true;
			}

			protected virtual bool PerformPosMoveRight()
			{
				var num = _hexBox._bytePos;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				var selectionLength = _hexBox._selectionLength;
				if (selectionLength != 0)
				{
					num += selectionLength;
					byteCharacterPos = 0;
					_hexBox.SetPosition(num, byteCharacterPos);
					_hexBox.ReleaseSelection();
				}
				else if (num != _hexBox._byteProvider.Length || byteCharacterPos != 0)
				{
					if (byteCharacterPos > 0)
					{
						num = Math.Min(_hexBox._byteProvider.Length, num + 1);
						byteCharacterPos = 0;
					}
					else
					{
						byteCharacterPos++;
					}
					_hexBox.SetPosition(num, byteCharacterPos);
					if (num > _hexBox._endByte - 1)
					{
						_hexBox.PerformScrollLineDown();
					}
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
				}
				_hexBox.ScrollByteIntoView();
				return true;
			}

			protected virtual bool PerformPosMoveLeftByte()
			{
				var bytePos = _hexBox._bytePos;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				if (bytePos == 0)
				{
					return true;
				}
				bytePos = Math.Max(0L, bytePos - 1);
				byteCharacterPos = 0;
				_hexBox.SetPosition(bytePos, byteCharacterPos);
				if (bytePos < _hexBox._startByte)
				{
					_hexBox.PerformScrollLineUp();
				}
				_hexBox.UpdateCaret();
				_hexBox.ScrollByteIntoView();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PerformPosMoveRightByte()
			{
				var bytePos = _hexBox._bytePos;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				if (bytePos == _hexBox._byteProvider.Length)
				{
					return true;
				}
				bytePos = Math.Min(_hexBox._byteProvider.Length, bytePos + 1);
				byteCharacterPos = 0;
				_hexBox.SetPosition(bytePos, byteCharacterPos);
				if (bytePos > _hexBox._endByte - 1)
				{
					_hexBox.PerformScrollLineDown();
				}
				_hexBox.UpdateCaret();
				_hexBox.ScrollByteIntoView();
				_hexBox.Invalidate();
				return true;
			}

			public virtual PointF GetCaretPointF(long byteIndex)
			{
				return _hexBox.GetBytePointF(byteIndex);
			}

			protected virtual BytePositionInfo GetBytePositionInfo(Point p)
			{
				return _hexBox.GetHexBytePositionInfo(p);
			}
		}

		private sealed class StringKeyInterpreter : KeyInterpreter
		{
			public StringKeyInterpreter(HexBox hexBox)
				: base(hexBox)
			{
				_hexBox._byteCharacterPos = 0;
			}

			public override bool PreProcessWmKeyDown(ref Message m)
			{
				var keys = (Keys)m.WParam.ToInt32();
				var keys2 = keys | ModifierKeys;
				var keys3 = keys2;
				if ((keys3 == Keys.Tab || keys3 == (Keys.Tab | Keys.Shift)) && RaiseKeyDown(keys2))
				{
					return true;
				}
				return keys2 switch
				{
					Keys.Tab | Keys.Shift => PreProcessWmKeyDown_ShiftTab(ref m), 
					Keys.Tab => PreProcessWmKeyDown_Tab(ref m), 
					_ => base.PreProcessWmKeyDown(ref m), 
				};
			}

			protected override bool PreProcessWmKeyDown_Left(ref Message m)
			{
				return PerformPosMoveLeftByte();
			}

			protected override bool PreProcessWmKeyDown_Right(ref Message m)
			{
				return PerformPosMoveRightByte();
			}

			public override bool PreProcessWmChar(ref Message m)
			{
				if (ModifierKeys == Keys.Control)
				{
					return _hexBox.BasePreProcessMessage(ref m);
				}
				var flag = _hexBox._byteProvider.SupportsWriteByte();
				var flag2 = _hexBox._byteProvider.SupportsInsertBytes();
				var flag3 = _hexBox._byteProvider.SupportsDeleteBytes();
				var bytePos = _hexBox._bytePos;
				var selectionLength = _hexBox._selectionLength;
				var byteCharacterPos = _hexBox._byteCharacterPos;
				if ((!flag && bytePos != _hexBox._byteProvider.Length) || (!flag2 && bytePos == _hexBox._byteProvider.Length))
				{
					return _hexBox.BasePreProcessMessage(ref m);
				}
				var c = (char)m.WParam.ToInt32();
				if (RaiseKeyPress(c))
				{
					return true;
				}
				if (_hexBox.ReadOnly)
				{
					return true;
				}
				var flag4 = bytePos == _hexBox._byteProvider.Length;
				if (!flag4 && flag2 && _hexBox.InsertActive)
				{
					flag4 = true;
				}
				if (flag3 && flag2 && selectionLength > 0)
				{
					_hexBox._byteProvider.DeleteBytes(bytePos, selectionLength);
					flag4 = true;
					byteCharacterPos = 0;
					_hexBox.SetPosition(bytePos, byteCharacterPos);
				}
				_hexBox.ReleaseSelection();
				var b = _hexBox.ByteCharConverter.ToByte(c);
				if (flag4)
				{
					_hexBox._byteProvider.InsertBytes(bytePos, new byte[1] { b });
				}
				else
				{
					_hexBox._byteProvider.WriteByte(bytePos, b);
				}
				PerformPosMoveRightByte();
				_hexBox.Invalidate();
				return true;
			}

			public override PointF GetCaretPointF(long byteIndex)
			{
				var gridBytePoint = _hexBox.GetGridBytePoint(byteIndex);
				return _hexBox.GetByteStringPointF(gridBytePoint);
			}

			protected override BytePositionInfo GetBytePositionInfo(Point p)
			{
				return _hexBox.GetStringBytePositionInfo(p);
			}
		}

		private const int THUMPTRACKDELAY = 50;

		private Rectangle _recContent;

		private Rectangle _recLineInfo;

		private Rectangle _recHex;

		private Rectangle _recStringView;

		private StringFormat _stringFormat;

		private SizeF _charSize;
        private int _iHexMaxVBytes;

		private int _iHexMaxBytes;

		private long _scrollVmin;

		private long _scrollVmax;

		private long _scrollVpos;

		private VScrollBar _vScrollBar;

		private System.Windows.Forms.Timer _thumbTrackTimer = new();

		private long _thumbTrackPosition;

		private int _lastThumbtrack;

		private int _recBorderLeft = SystemInformation.Border3DSize.Width;

		private int _recBorderRight = SystemInformation.Border3DSize.Width;

		private int _recBorderTop = SystemInformation.Border3DSize.Height;

		private int _recBorderBottom = SystemInformation.Border3DSize.Height;

		private long _startByte;

		private long _endByte;

		private long _bytePos = -1L;

		private int _byteCharacterPos;

		private string _hexStringFormat = "X";

		private IKeyInterpreter _keyInterpreter;

		private EmptyKeyInterpreter _eki;

		private KeyInterpreter _ki;

		private StringKeyInterpreter _ski;

		private bool _caretVisible;

		private bool _abortFind;
        private bool _insertActive;
        private Color _backColorDisabled = Color.FromName("WhiteSmoke");

		private bool _readOnly;

		private int _bytesPerLine = 16;

		private bool _useFixedBytesPerLine;

		private bool _vScrollBarVisible;

		private IByteProvider _byteProvider;

		private bool _lineInfoVisible;

		private long _lineInfoOffset;

		private BorderStyle _borderStyle = BorderStyle.Fixed3D;

		private bool _stringViewVisible;

		private long _selectionLength;

		private Color _lineInfoForeColor = Color.Empty;

		private Color _selectionBackColor = Color.Blue;

		private Color _selectionForeColor = Color.White;

		private bool _shadowSelectionVisible = true;

		private Color _shadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
        private int _currentPositionInLine;
        private IByteCharConverter _byteCharConverter;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public long CurrentFindingPosition { get; private set; }

        public byte LineInfoDigits { get; set; } = 2;

        [DefaultValue(typeof(Color), "White")]
		public sealed override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		public sealed override Font Font
		{
			get => base.Font;
			set => base.Font = value;
		}

		[Browsable(false)]
		[Bindable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text
		{
			get => base.Text;
			set => base.Text = value;
		}

		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft
		{
			get => base.RightToLeft;
			set => base.RightToLeft = value;
		}

		[DefaultValue(typeof(Color), "WhiteSmoke")]
		[Category("Appearance")]
		public Color BackColorDisabled
		{
			get => _backColorDisabled;
			set => _backColorDisabled = value;
		}

		[Category("Hex")]
		[Description("Gets or sets if the count of bytes in one line is fix.")]
		[DefaultValue(false)]
		public bool ReadOnly
		{
			get => _readOnly;
			set
			{
				if (_readOnly != value)
				{
					_readOnly = value;
					OnReadOnlyChanged(EventArgs.Empty);
					Invalidate();
				}
			}
		}

		[DefaultValue(16)]
		[Category("Hex")]
		[Description("Gets or sets the maximum count of bytes in one line.")]
		public int BytesPerLine
		{
			get => _bytesPerLine;
			set
			{
				if (_bytesPerLine != value)
				{
					_bytesPerLine = value;
					OnBytesPerLineChanged(EventArgs.Empty);
					UpdateRectanglePositioning();
					Invalidate();
				}
			}
		}

		[Category("Hex")]
		[Description("Gets or sets if the count of bytes in one line is fix.")]
		[DefaultValue(false)]
		public bool UseFixedBytesPerLine
		{
			get => _useFixedBytesPerLine;
			set
			{
				if (_useFixedBytesPerLine != value)
				{
					_useFixedBytesPerLine = value;
					OnUseFixedBytesPerLineChanged(EventArgs.Empty);
					UpdateRectanglePositioning();
					Invalidate();
				}
			}
		}

		[DefaultValue(false)]
		[Description("Gets or sets the visibility of a vertical scroll bar.")]
		[Category("Hex")]
		public bool VScrollBarVisible
		{
			get => _vScrollBarVisible;
			set
			{
				if (_vScrollBarVisible != value)
				{
					_vScrollBarVisible = value;
					if (_vScrollBarVisible)
					{
						Controls.Add(_vScrollBar);
					}
					else
					{
						Controls.Remove(_vScrollBar);
					}
					UpdateRectanglePositioning();
					UpdateScrollSize();
					OnVScrollBarVisibleChanged(EventArgs.Empty);
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IByteProvider ByteProvider
		{
			get => _byteProvider;
			set
			{
				if (_byteProvider == value)
				{
					return;
				}
				if (value == null)
				{
					ActivateEmptyKeyInterpreter();
				}
				else
				{
					ActivateKeyInterpreter();
				}
				if (_byteProvider != null)
				{
					_byteProvider.LengthChanged -= _byteProvider_LengthChanged;
				}
				_byteProvider = value;
				if (_byteProvider != null)
				{
					_byteProvider.LengthChanged += _byteProvider_LengthChanged;
				}
				OnByteProviderChanged(EventArgs.Empty);
				if (value == null)
				{
					_bytePos = -1L;
					_byteCharacterPos = 0;
					_selectionLength = 0L;
					DestroyCaret();
				}
				else
				{
					SetPosition(0L, 0);
					SetSelectionLength(0L);
					if (_caretVisible && Focused)
					{
						UpdateCaret();
					}
					else
					{
						CreateCaret();
					}
				}
				CheckCurrentLineChanged();
				CheckCurrentPositionInLineChanged();
				_scrollVpos = 0L;
				UpdateVisibilityBytes();
				UpdateRectanglePositioning();
				Invalidate();
			}
		}

		[DefaultValue(false)]
		[Description("Gets or sets the visibility of a line info.")]
		[Category("Hex")]
		public bool LineInfoVisible
		{
			get => _lineInfoVisible;
			set
			{
				if (_lineInfoVisible != value)
				{
					_lineInfoVisible = value;
					OnLineInfoVisibleChanged(EventArgs.Empty);
					UpdateRectanglePositioning();
					Invalidate();
				}
			}
		}

		[Description("Gets or sets the offset of the line info.")]
		[DefaultValue(0L)]
		[Category("Hex")]
		public long LineInfoOffset
		{
			get => _lineInfoOffset;
			set
			{
				if (_lineInfoOffset != value)
				{
					_lineInfoOffset = value;
					Invalidate();
				}
			}
		}

		[DefaultValue(typeof(BorderStyle), "Fixed3D")]
		[Description("Gets or sets the hex box\u00b4s border style.")]
		[Category("Hex")]
		public BorderStyle BorderStyle
		{
			get => _borderStyle;
			set
			{
				if (_borderStyle != value)
				{
					_borderStyle = value;
					switch (_borderStyle)
					{
					case BorderStyle.None:
						_recBorderLeft = (_recBorderTop = (_recBorderRight = (_recBorderBottom = 0)));
						break;
					case BorderStyle.Fixed3D:
						_recBorderLeft = (_recBorderRight = SystemInformation.Border3DSize.Width);
						_recBorderTop = (_recBorderBottom = SystemInformation.Border3DSize.Height);
						break;
					case BorderStyle.FixedSingle:
						_recBorderLeft = (_recBorderTop = (_recBorderRight = (_recBorderBottom = 1)));
						break;
					}
					UpdateRectanglePositioning();
					OnBorderStyleChanged(EventArgs.Empty);
				}
			}
		}

		[Description("Gets or sets the visibility of the string view.")]
		[DefaultValue(false)]
		[Category("Hex")]
		public bool StringViewVisible
		{
			get => _stringViewVisible;
			set
			{
				if (_stringViewVisible != value)
				{
					_stringViewVisible = value;
					OnStringViewVisibleChanged(EventArgs.Empty);
					UpdateRectanglePositioning();
					Invalidate();
				}
			}
		}

		[Category("Hex")]
		[DefaultValue(typeof(HexCasing), "Upper")]
		[Description("Gets or sets whether the HexBox control displays the hex characters in upper or lower case.")]
		public HexCasing HexCasing
		{
			get
			{
				if (_hexStringFormat == "X")
				{
					return HexCasing.Upper;
				}
				return HexCasing.Lower;
			}
			set
			{
				var text = ((value != 0) ? "x" : "X");
				if (!(_hexStringFormat == text))
				{
					_hexStringFormat = text;
					OnHexCasingChanged(EventArgs.Empty);
					Invalidate();
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long SelectionStart
		{
			get => _bytePos;
			set
			{
				SetPosition(value, 0);
				ScrollByteIntoView();
				Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long SelectionLength
		{
			get => _selectionLength;
			set
			{
				SetSelectionLength(value);
				ScrollByteIntoView();
				Invalidate();
			}
		}

		[Category("Hex")]
		[DefaultValue(typeof(Color), "Empty")]
		[Description("Gets or sets the line info color. When this property is null, then ForeColor property is used.")]
		public Color LineInfoForeColor
		{
			get => _lineInfoForeColor;
			set
			{
				_lineInfoForeColor = value;
				Invalidate();
			}
		}

		[Description("Gets or sets the background color for the selected bytes.")]
		[DefaultValue(typeof(Color), "Blue")]
		[Category("Hex")]
		public Color SelectionBackColor
		{
			get => _selectionBackColor;
			set
			{
				_selectionBackColor = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[Description("Gets or sets the foreground color for the selected bytes.")]
		[Category("Hex")]
		public Color SelectionForeColor
		{
			get => _selectionForeColor;
			set
			{
				_selectionForeColor = value;
				Invalidate();
			}
		}

		[Category("Hex")]
		[DefaultValue(true)]
		[Description("Gets or sets the visibility of a shadow selection.")]
		public bool ShadowSelectionVisible
		{
			get => _shadowSelectionVisible;
			set
			{
				if (_shadowSelectionVisible != value)
				{
					_shadowSelectionVisible = value;
					Invalidate();
				}
			}
		}

		[Description("Gets or sets the color of the shadow selection.")]
		[Category("Hex")]
		public Color ShadowSelectionColor
		{
			get => _shadowSelectionColor;
			set
			{
				_shadowSelectionColor = value;
				Invalidate();
			}
		}

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HorizontalByteCount { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int VerticalByteCount => _iHexMaxVBytes;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public long CurrentLine { get; private set; }

        [Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long CurrentPositionInLine => _currentPositionInLine;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool InsertActive
		{
			get => _insertActive;
			set
			{
				if (_insertActive != value)
				{
					_insertActive = value;
					DestroyCaret();
					CreateCaret();
					OnInsertActiveChanged(EventArgs.Empty);
				}
			}
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BuiltInContextMenu BuiltInContextMenu { get; }

        [Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IByteCharConverter ByteCharConverter
		{
			get
			{
				if (_byteCharConverter == null)
				{
					_byteCharConverter = new DefaultByteCharConverter();
				}
				return _byteCharConverter;
			}
			set
			{
				if (value != null && value != _byteCharConverter)
				{
					_byteCharConverter = value;
					Invalidate();
				}
			}
		}

		[Description("Occurs, when the value of InsertActive property has changed.")]
		public event EventHandler InsertActiveChanged;

		[Description("Occurs, when the value of ReadOnly property has changed.")]
		public event EventHandler ReadOnlyChanged;

		[Description("Occurs, when the value of ByteProvider property has changed.")]
		public event EventHandler ByteProviderChanged;

		[Description("Occurs, when the value of SelectionStart property has changed.")]
		public event EventHandler SelectionStartChanged;

		[Description("Occurs, when the value of SelectionLength property has changed.")]
		public event EventHandler SelectionLengthChanged;

		[Description("Occurs, when the value of LineInfoVisible property has changed.")]
		public event EventHandler LineInfoVisibleChanged;

		[Description("Occurs, when the value of StringViewVisible property has changed.")]
		public event EventHandler StringViewVisibleChanged;

		[Description("Occurs, when the value of BorderStyle property has changed.")]
		public event EventHandler BorderStyleChanged;

		[Description("Occurs, when the value of BytesPerLine property has changed.")]
		public event EventHandler BytesPerLineChanged;

		[Description("Occurs, when the value of UseFixedBytesPerLine property has changed.")]
		public event EventHandler UseFixedBytesPerLineChanged;

		[Description("Occurs, when the value of VScrollBarVisible property has changed.")]
		public event EventHandler VScrollBarVisibleChanged;

		[Description("Occurs, when the value of HexCasing property has changed.")]
		public event EventHandler HexCasingChanged;

		[Description("Occurs, when the value of HorizontalByteCount property has changed.")]
		public event EventHandler HorizontalByteCountChanged;

		[Description("Occurs, when the value of VerticalByteCount property has changed.")]
		public event EventHandler VerticalByteCountChanged;

		[Description("Occurs, when the value of CurrentLine property has changed.")]
		public event EventHandler CurrentLineChanged;

		[Description("Occurs, when the value of CurrentPositionInLine property has changed.")]
		public event EventHandler CurrentPositionInLineChanged;

		[Description("Occurs, when Copy method was invoked and ClipBoardData changed.")]
		public event EventHandler Copied;

		[Description("Occurs, when CopyHex method was invoked and ClipBoardData changed.")]
		public event EventHandler CopiedHex;

		public HexBox()
		{
			_vScrollBar = new VScrollBar();
			_vScrollBar.Scroll += _vScrollBar_Scroll;
			BuiltInContextMenu = new BuiltInContextMenu(this);
			BackColor = Color.White;
			Font = new Font("Courier New", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            _stringFormat = new StringFormat(StringFormat.GenericTypographic)
            {
                FormatFlags = StringFormatFlags.MeasureTrailingSpaces
            };
            ActivateEmptyKeyInterpreter();
			SetStyle(ControlStyles.UserPaint, value: true);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
			_thumbTrackTimer.Interval = 50;
			_thumbTrackTimer.Tick += PerformScrollThumbTrack;
		}

		private void _vScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			switch (e.Type)
			{
			case ScrollEventType.SmallIncrement:
				PerformScrollLineDown();
				break;
			case ScrollEventType.SmallDecrement:
				PerformScrollLineUp();
				break;
			case ScrollEventType.LargeIncrement:
				PerformScrollPageDown();
				break;
			case ScrollEventType.LargeDecrement:
				PerformScrollPageUp();
				break;
			case ScrollEventType.ThumbPosition:
			{
				var pos = FromScrollPos(e.NewValue);
				PerformScrollThumpPosition(pos);
				break;
			}
			case ScrollEventType.ThumbTrack:
			{
				if (_thumbTrackTimer.Enabled)
				{
					_thumbTrackTimer.Enabled = false;
				}
				var tickCount = Environment.TickCount;
				if (tickCount - _lastThumbtrack > 50)
				{
					PerformScrollThumbTrack(null, null);
					_lastThumbtrack = tickCount;
				}
				else
				{
					_thumbTrackPosition = FromScrollPos(e.NewValue);
					_thumbTrackTimer.Enabled = true;
				}
				break;
			}
			}
			e.NewValue = ToScrollPos(_scrollVpos);
		}

		private void PerformScrollThumbTrack(object sender, EventArgs e)
		{
			_thumbTrackTimer.Enabled = false;
			PerformScrollThumpPosition(_thumbTrackPosition);
			_lastThumbtrack = Environment.TickCount;
		}

		public void UpdateScrollSize()
		{
			if (VScrollBarVisible && _byteProvider != null && _byteProvider.Length > 0 && HorizontalByteCount != 0)
			{
				var val = (long)Math.Ceiling((_byteProvider.Length + 1) / (double)HorizontalByteCount - _iHexMaxVBytes);
				val = Math.Max(0L, val);
				var num = _startByte / HorizontalByteCount;
				if (val < _scrollVmax && _scrollVpos == _scrollVmax)
				{
					PerformScrollLineUp();
				}
				if (val != _scrollVmax || num != _scrollVpos)
				{
					_scrollVmin = 0L;
					_scrollVmax = val;
					_scrollVpos = Math.Min(num, val);
					UpdateVScroll();
				}
			}
			else if (VScrollBarVisible)
			{
				_scrollVmin = 0L;
				_scrollVmax = 0L;
				_scrollVpos = 0L;
				UpdateVScroll();
			}
		}

		private void UpdateVScroll()
		{
			var num = ToScrollMax(_scrollVmax);
			if (num > 0)
			{
				_vScrollBar.Minimum = 0;
				_vScrollBar.Maximum = num;
				_vScrollBar.Value = ToScrollPos(_scrollVpos);
				_vScrollBar.Enabled = true;
			}
			else
			{
				_vScrollBar.Enabled = false;
			}
		}

		private int ToScrollPos(long value)
		{
			var num = 65535;
			if (_scrollVmax < num)
			{
				return (int)value;
			}
			var num2 = value / (double)_scrollVmax * 100.0;
			var num3 = (int)Math.Floor(num / 100.0 * num2);
			num3 = (int)Math.Max(_scrollVmin, num3);
			return (int)Math.Min(_scrollVmax, num3);
		}

		private long FromScrollPos(int value)
		{
			var num = 65535;
			if (_scrollVmax < num)
			{
				return value;
			}
			var num2 = value / (double)num * 100.0;
			return (int)Math.Floor(_scrollVmax / 100.0 * num2);
		}

		private int ToScrollMax(long value)
		{
			var num = 65535L;
			if (value > num)
			{
				return (int)num;
			}
			return (int)value;
		}

		private void PerformScrollToLine(long pos)
		{
			if (pos >= _scrollVmin && pos <= _scrollVmax && pos != _scrollVpos)
			{
				_scrollVpos = pos;
				UpdateVScroll();
				UpdateVisibilityBytes();
				UpdateCaret();
				Invalidate();
			}
		}

		private void PerformScrollLines(int lines)
		{
			long pos;
			if (lines > 0)
			{
				pos = Math.Min(_scrollVmax, _scrollVpos + lines);
			}
			else
			{
				if (lines >= 0)
				{
					return;
				}
				pos = Math.Max(_scrollVmin, _scrollVpos + lines);
			}
			PerformScrollToLine(pos);
		}

		private void PerformScrollLineDown()
		{
			PerformScrollLines(1);
		}

		private void PerformScrollLineUp()
		{
			PerformScrollLines(-1);
		}

		private void PerformScrollPageDown()
		{
			PerformScrollLines(_iHexMaxVBytes);
		}

		private void PerformScrollPageUp()
		{
			PerformScrollLines(-_iHexMaxVBytes);
		}

		private void PerformScrollThumpPosition(long pos)
		{
			var num = ((_scrollVmax > 65535) ? 10 : 9);
			if (ToScrollPos(pos) == ToScrollMax(_scrollVmax) - num)
			{
				pos = _scrollVmax;
			}
			PerformScrollToLine(pos);
		}

		public void ScrollByteIntoView()
		{
			ScrollByteIntoView(_bytePos);
		}

		public void ScrollByteIntoView(long index)
		{
			if (_byteProvider != null && _keyInterpreter != null)
			{
				if (index < _startByte)
				{
					var pos = (long)Math.Floor(index / (double)HorizontalByteCount);
					PerformScrollThumpPosition(pos);
				}
				else if (index > _endByte)
				{
					var num = (long)Math.Floor(index / (double)HorizontalByteCount);
					num -= _iHexMaxVBytes - 1;
					PerformScrollThumpPosition(num);
				}
			}
		}

		private void ReleaseSelection()
		{
			if (_selectionLength != 0)
			{
				_selectionLength = 0L;
				OnSelectionLengthChanged(EventArgs.Empty);
				if (!_caretVisible)
				{
					CreateCaret();
				}
				else
				{
					UpdateCaret();
				}
				Invalidate();
			}
		}

		public bool CanSelectAll()
		{
			if (!Enabled)
			{
				return false;
			}
			if (_byteProvider == null)
			{
				return false;
			}
			return true;
		}

		public void SelectAll()
		{
			if (ByteProvider != null)
			{
				Select(0L, ByteProvider.Length);
			}
		}

		public void Select(long start, long length)
		{
			if (ByteProvider != null && Enabled)
			{
				InternalSelect(start, length);
				ScrollByteIntoView();
			}
		}

		private void InternalSelect(long start, long length)
		{
			var byteCharacterPos = 0;
			if (length > 0 && _caretVisible)
			{
				DestroyCaret();
			}
			else if (length == 0 && !_caretVisible)
			{
				CreateCaret();
			}
			SetPosition(start, byteCharacterPos);
			SetSelectionLength(length);
			UpdateCaret();
			Invalidate();
		}

		private void ActivateEmptyKeyInterpreter()
		{
			if (_eki == null)
			{
				_eki = new EmptyKeyInterpreter(this);
			}
			if (_eki != _keyInterpreter)
			{
				if (_keyInterpreter != null)
				{
					_keyInterpreter.Deactivate();
				}
				_keyInterpreter = _eki;
				_keyInterpreter.Activate();
			}
		}

		private void ActivateKeyInterpreter()
		{
			if (_ki == null)
			{
				_ki = new KeyInterpreter(this);
			}
			if (_ki != _keyInterpreter)
			{
				if (_keyInterpreter != null)
				{
					_keyInterpreter.Deactivate();
				}
				_keyInterpreter = _ki;
				_keyInterpreter.Activate();
			}
		}

		private void ActivateStringKeyInterpreter()
		{
			if (_ski == null)
			{
				_ski = new StringKeyInterpreter(this);
			}
			if (_ski != _keyInterpreter)
			{
				if (_keyInterpreter != null)
				{
					_keyInterpreter.Deactivate();
				}
				_keyInterpreter = _ski;
				_keyInterpreter.Activate();
			}
		}

		private void CreateCaret()
		{
			if (_byteProvider != null && _keyInterpreter != null && !_caretVisible && Focused)
			{
				var nWidth = (InsertActive ? 1 : ((int)_charSize.Width));
				var nHeight = (int)_charSize.Height;
				NativeMethods.CreateCaret(Handle, IntPtr.Zero, nWidth, nHeight);
				UpdateCaret();
				NativeMethods.ShowCaret(Handle);
				_caretVisible = true;
			}
		}

		private void UpdateCaret()
		{
			if (_byteProvider != null && _keyInterpreter != null)
			{
				var byteIndex = _bytePos - _startByte;
				var caretPointF = _keyInterpreter.GetCaretPointF(byteIndex);
				caretPointF.X += _byteCharacterPos * _charSize.Width;
				NativeMethods.SetCaretPos((int)caretPointF.X, (int)caretPointF.Y);
			}
		}

		private void DestroyCaret()
		{
			if (_caretVisible)
			{
				NativeMethods.DestroyCaret();
				_caretVisible = false;
			}
		}

		private void SetCaretPosition(Point p)
		{
			if (_byteProvider != null && _keyInterpreter != null)
			{
				var bytePos = _bytePos;
				var byteCharacterPos = _byteCharacterPos;
				if (_recHex.Contains(p))
				{
					var hexBytePositionInfo = GetHexBytePositionInfo(p);
					bytePos = hexBytePositionInfo.Index;
					byteCharacterPos = hexBytePositionInfo.CharacterPosition;
					SetPosition(bytePos, byteCharacterPos);
					ActivateKeyInterpreter();
					UpdateCaret();
					Invalidate();
				}
				else if (_recStringView.Contains(p))
				{
					var stringBytePositionInfo = GetStringBytePositionInfo(p);
					bytePos = stringBytePositionInfo.Index;
					byteCharacterPos = stringBytePositionInfo.CharacterPosition;
					SetPosition(bytePos, byteCharacterPos);
					ActivateStringKeyInterpreter();
					UpdateCaret();
					Invalidate();
				}
			}
		}

		private BytePositionInfo GetHexBytePositionInfo(Point p)
		{
			var num = (p.X - _recHex.X) / _charSize.Width;
			var num2 = (p.Y - _recHex.Y) / _charSize.Height;
			var num3 = (int)num;
			var num4 = (int)num2;
			var num5 = num3 / 3 + 1;
			var num6 = Math.Min(_byteProvider.Length, _startByte + (HorizontalByteCount * (num4 + 1) - HorizontalByteCount) + num5 - 1);
			var num7 = num3 % 3;
			if (num7 > 1)
			{
				num7 = 1;
			}
			if (num6 == _byteProvider.Length)
			{
				num7 = 0;
			}
			if (num6 < 0)
			{
				return new BytePositionInfo(0L, 0);
			}
			return new BytePositionInfo(num6, num7);
		}

		private BytePositionInfo GetStringBytePositionInfo(Point p)
		{
			var num = (p.X - _recStringView.X) / _charSize.Width;
			var num2 = (p.Y - _recStringView.Y) / _charSize.Height;
			var num3 = (int)num;
			var num4 = (int)num2;
			var num5 = num3 + 1;
			var num6 = Math.Min(_byteProvider.Length, _startByte + (HorizontalByteCount * (num4 + 1) - HorizontalByteCount) + num5 - 1);
			var characterPosition = 0;
			if (num6 < 0)
			{
				return new BytePositionInfo(0L, 0);
			}
			return new BytePositionInfo(num6, characterPosition);
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		[SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
		public override bool PreProcessMessage(ref Message m)
		{
			return m.Msg switch
			{
				256 => _keyInterpreter.PreProcessWmKeyDown(ref m), 
				258 => _keyInterpreter.PreProcessWmChar(ref m), 
				257 => _keyInterpreter.PreProcessWmKeyUp(ref m), 
				_ => base.PreProcessMessage(ref m), 
			};
		}

		private bool BasePreProcessMessage(ref Message m)
		{
			return base.PreProcessMessage(ref m);
		}

		public long Find(byte[] bytes, long startIndex)
		{
			var num = 0;
			var num2 = bytes.Length;
			_abortFind = false;
			for (var num3 = startIndex; num3 < _byteProvider.Length; num3++)
			{
				if (_abortFind)
				{
					return -2L;
				}
				if (num3 % 1000 == 0)
				{
					Application.DoEvents();
				}
				if (_byteProvider.ReadByte(num3) != bytes[num])
				{
					num3 -= num;
					num = 0;
					CurrentFindingPosition = num3;
					continue;
				}
				num++;
				if (num == num2)
				{
					var num4 = num3 - num2 + 1;
					Select(num4, num2);
					ScrollByteIntoView(_bytePos + _selectionLength);
					ScrollByteIntoView(_bytePos);
					return num4;
				}
			}
			return -1L;
		}

		public void AbortFind()
		{
			_abortFind = true;
		}

		private byte[] GetCopyData()
		{
			if (!CanCopy())
			{
				return new byte[0];
			}
			var array = new byte[_selectionLength];
			var num = -1;
			for (var num2 = _bytePos; num2 < _bytePos + _selectionLength; num2++)
			{
				num++;
				array[num] = _byteProvider.ReadByte(num2);
			}
			return array;
		}

		public void Copy()
		{
			if (CanCopy())
			{
				var copyData = GetCopyData();
				var dataObject = new DataObject();
				var @string = Encoding.ASCII.GetString(copyData, 0, copyData.Length);
				dataObject.SetData(typeof(string), @string);
				var data = new MemoryStream(copyData, 0, copyData.Length, writable: false, publiclyVisible: true);
				dataObject.SetData("BinaryData", data);
				Clipboard.SetDataObject(dataObject, copy: true);
				UpdateCaret();
				ScrollByteIntoView();
				Invalidate();
				OnCopied(EventArgs.Empty);
			}
		}

		public bool CanCopy()
		{
			if (_selectionLength < 1 || _byteProvider == null)
			{
				return false;
			}
			return true;
		}

		public void Cut()
		{
			if (CanCut())
			{
				Copy();
				_byteProvider.DeleteBytes(_bytePos, _selectionLength);
				_byteCharacterPos = 0;
				UpdateCaret();
				ScrollByteIntoView();
				ReleaseSelection();
				Invalidate();
				Refresh();
			}
		}

		public bool CanCut()
		{
			if (ReadOnly || !Enabled)
			{
				return false;
			}
			if (_byteProvider == null)
			{
				return false;
			}
			if (_selectionLength < 1 || !_byteProvider.SupportsDeleteBytes())
			{
				return false;
			}
			return true;
		}

		public void Paste()
		{
			if (!CanPaste())
			{
				return;
			}
			if (_selectionLength > 0)
			{
				_byteProvider.DeleteBytes(_bytePos, _selectionLength);
			}
			byte[] array = null;
			var dataObject = Clipboard.GetDataObject();
			if (dataObject.GetDataPresent("BinaryData"))
			{
				var memoryStream = (MemoryStream)dataObject.GetData("BinaryData");
				array = new byte[memoryStream.Length];
				memoryStream.Read(array, 0, array.Length);
			}
			else
			{
				if (!dataObject.GetDataPresent(typeof(string)))
				{
					return;
				}
				var s = (string)dataObject.GetData(typeof(string));
				array = Encoding.ASCII.GetBytes(s);
			}
			_byteProvider.InsertBytes(_bytePos, array);
			SetPosition(_bytePos + array.Length, 0);
			ReleaseSelection();
			ScrollByteIntoView();
			UpdateCaret();
			Invalidate();
		}

		public bool CanPaste()
		{
			if (ReadOnly || !Enabled)
			{
				return false;
			}
			if (_byteProvider == null || !_byteProvider.SupportsInsertBytes())
			{
				return false;
			}
			if (!_byteProvider.SupportsDeleteBytes() && _selectionLength > 0)
			{
				return false;
			}
			var dataObject = Clipboard.GetDataObject();
			if (dataObject.GetDataPresent("BinaryData"))
			{
				return true;
			}
			if (dataObject.GetDataPresent(typeof(string)))
			{
				return true;
			}
			return false;
		}

		public bool CanPasteHex()
		{
			if (!CanPaste())
			{
				return false;
			}
			byte[] array = null;
			var dataObject = Clipboard.GetDataObject();
			if (dataObject.GetDataPresent(typeof(string)))
			{
				var hex = (string)dataObject.GetData(typeof(string));
				array = ConvertHexToBytes(hex);
				return array != null;
			}
			return false;
		}

		public void PasteHex()
		{
			if (!CanPaste())
			{
				return;
			}
			byte[] array = null;
			var dataObject = Clipboard.GetDataObject();
			if (!dataObject.GetDataPresent(typeof(string)))
			{
				return;
			}
			var hex = (string)dataObject.GetData(typeof(string));
			array = ConvertHexToBytes(hex);
			if (array != null)
			{
				if (_selectionLength > 0)
				{
					_byteProvider.DeleteBytes(_bytePos, _selectionLength);
				}
				_byteProvider.InsertBytes(_bytePos, array);
				SetPosition(_bytePos + array.Length, 0);
				ReleaseSelection();
				ScrollByteIntoView();
				UpdateCaret();
				Invalidate();
			}
		}

		public void CopyHex()
		{
			if (CanCopy())
			{
				var copyData = GetCopyData();
				var dataObject = new DataObject();
				var data = ConvertBytesToHex(copyData);
				dataObject.SetData(typeof(string), data);
				var data2 = new MemoryStream(copyData, 0, copyData.Length, writable: false, publiclyVisible: true);
				dataObject.SetData("BinaryData", data2);
				Clipboard.SetDataObject(dataObject, copy: true);
				UpdateCaret();
				ScrollByteIntoView();
				Invalidate();
				OnCopiedHex(EventArgs.Empty);
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			switch (_borderStyle)
			{
			case BorderStyle.Fixed3D:
				if (TextBoxRenderer.IsSupported)
				{
					var element = VisualStyleElement.TextBox.TextEdit.Normal;
					var color = BackColor;
					if (Enabled)
					{
						if (ReadOnly)
						{
							element = VisualStyleElement.TextBox.TextEdit.ReadOnly;
						}
						else if (Focused)
						{
							element = VisualStyleElement.TextBox.TextEdit.Focused;
						}
					}
					else
					{
						element = VisualStyleElement.TextBox.TextEdit.Disabled;
						color = BackColorDisabled;
					}
					var visualStyleRenderer = new VisualStyleRenderer(element);
					visualStyleRenderer.DrawBackground(e.Graphics, ClientRectangle);
					var backgroundContentRectangle = visualStyleRenderer.GetBackgroundContentRectangle(e.Graphics, ClientRectangle);
					e.Graphics.FillRectangle(new SolidBrush(color), backgroundContentRectangle);
				}
				else
				{
					e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
					ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Sunken);
				}
				break;
			case BorderStyle.FixedSingle:
				e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
				ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
				break;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_byteProvider == null)
			{
				return;
			}
			var region = new Region(ClientRectangle);
			region.Exclude(_recContent);
			e.Graphics.ExcludeClip(region);
			UpdateVisibilityBytes();
			if (_lineInfoVisible)
			{
				PaintLineInfo(e.Graphics, _startByte, _endByte);
			}
			if (!_stringViewVisible)
			{
				PaintHex(e.Graphics, _startByte, _endByte);
				return;
			}
			PaintHexAndStringView(e.Graphics, _startByte, _endByte);
			if (_shadowSelectionVisible)
			{
				PaintCurrentBytesSign(e.Graphics);
			}
		}

		private void PaintLineInfo(Graphics g, long startByte, long endByte)
		{
			endByte = Math.Min(_byteProvider.Length - 1, endByte);
			var color = ((LineInfoForeColor != Color.Empty) ? LineInfoForeColor : ForeColor);
			Brush brush = new SolidBrush(color);
			var num = GetGridBytePoint(endByte - startByte).Y + 1;
			for (var i = 0; i < num; i++)
			{
				var num2 = startByte + HorizontalByteCount * i + _lineInfoOffset;
				var bytePointF = GetBytePointF(new Point(0, i));
				var text = num2.ToString(_hexStringFormat, Thread.CurrentThread.CurrentCulture);
				var num3 = 8 - text.Length;
				var s = ((num3 <= -1) ? new string('~', LineInfoDigits) : (new string('0', LineInfoDigits - text.Length) + text));
				g.DrawString(s, Font, brush, new PointF(_recLineInfo.X, bytePointF.Y), _stringFormat);
			}
		}

		private void PaintHex(Graphics g, long startByte, long endByte)
		{
			Brush brush = new SolidBrush(GetDefaultForeColor());
			Brush brush2 = new SolidBrush(_selectionForeColor);
			Brush brushBack = new SolidBrush(_selectionBackColor);
			var num = -1;
			var num2 = Math.Min(_byteProvider.Length - 1, endByte + HorizontalByteCount);
			var flag = _keyInterpreter == null || _keyInterpreter.GetType() == typeof(KeyInterpreter);
			for (var num3 = startByte; num3 < num2 + 1; num3++)
			{
				num++;
				var gridBytePoint = GetGridBytePoint(num);
				var b = _byteProvider.ReadByte(num3);
				if (num3 >= _bytePos && num3 <= _bytePos + _selectionLength - 1 && _selectionLength != 0 && flag)
				{
					PaintHexStringSelected(g, b, brush2, brushBack, gridBytePoint);
				}
				else
				{
					PaintHexString(g, b, brush, gridBytePoint);
				}
			}
		}

		private void PaintHexString(Graphics g, byte b, Brush brush, Point gridPoint)
		{
			var bytePointF = GetBytePointF(gridPoint);
			var text = ConvertByteToHex(b);
			g.DrawString(text.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
			bytePointF.X += _charSize.Width;
			g.DrawString(text.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
		}

		private void PaintHexStringSelected(Graphics g, byte b, Brush brush, Brush brushBack, Point gridPoint)
		{
			var text = b.ToString(_hexStringFormat, Thread.CurrentThread.CurrentCulture);
			if (text.Length == 1)
			{
				text = "0" + text;
			}
			var bytePointF = GetBytePointF(gridPoint);
			var num = ((gridPoint.X + 1 == HorizontalByteCount) ? (_charSize.Width * 2f) : (_charSize.Width * 3f));
			g.FillRectangle(brushBack, bytePointF.X, bytePointF.Y, num, _charSize.Height);
			g.DrawString(text.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
			bytePointF.X += _charSize.Width;
			g.DrawString(text.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
		}

		private void PaintHexAndStringView(Graphics g, long startByte, long endByte)
		{
			Brush brush = new SolidBrush(GetDefaultForeColor());
			Brush brush2 = new SolidBrush(_selectionForeColor);
			Brush brush3 = new SolidBrush(_selectionBackColor);
			var num = -1;
			var num2 = Math.Min(_byteProvider.Length - 1, endByte + HorizontalByteCount);
			var flag = _keyInterpreter == null || _keyInterpreter.GetType() == typeof(KeyInterpreter);
			var flag2 = _keyInterpreter is StringKeyInterpreter;
			for (var num3 = startByte; num3 < num2 + 1; num3++)
			{
				num++;
				var gridBytePoint = GetGridBytePoint(num);
				var byteStringPointF = GetByteStringPointF(gridBytePoint);
				var b = _byteProvider.ReadByte(num3);
				var flag3 = num3 >= _bytePos && num3 <= _bytePos + _selectionLength - 1 && _selectionLength != 0;
				if (flag3 && flag)
				{
					PaintHexStringSelected(g, b, brush2, brush3, gridBytePoint);
				}
				else
				{
					PaintHexString(g, b, brush, gridBytePoint);
				}
				var s = new string(ByteCharConverter.ToChar(b), 1);
				if (flag3 && flag2)
				{
					g.FillRectangle(brush3, byteStringPointF.X, byteStringPointF.Y, _charSize.Width, _charSize.Height);
					g.DrawString(s, Font, brush2, byteStringPointF, _stringFormat);
				}
				else
				{
					g.DrawString(s, Font, brush, byteStringPointF, _stringFormat);
				}
			}
		}

		private void PaintCurrentBytesSign(Graphics g)
		{
			if (_keyInterpreter == null || !Focused || _bytePos == -1 || !Enabled)
			{
				return;
			}
			if (_keyInterpreter.GetType() == typeof(KeyInterpreter))
			{
				if (_selectionLength == 0)
				{
					var gridBytePoint = GetGridBytePoint(_bytePos - _startByte);
					var byteStringPointF = GetByteStringPointF(gridBytePoint);
					var size = new Size((int)_charSize.Width, (int)_charSize.Height);
					var rec = new Rectangle((int)byteStringPointF.X, (int)byteStringPointF.Y, size.Width, size.Height);
					if (rec.IntersectsWith(_recStringView))
					{
						rec.Intersect(_recStringView);
						PaintCurrentByteSign(g, rec);
					}
					return;
				}
				var num = (int)(_recStringView.Width - _charSize.Width);
				var gridBytePoint2 = GetGridBytePoint(_bytePos - _startByte);
				var byteStringPointF2 = GetByteStringPointF(gridBytePoint2);
				var gridBytePoint3 = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1);
				var byteStringPointF3 = GetByteStringPointF(gridBytePoint3);
				var num2 = gridBytePoint3.Y - gridBytePoint2.Y;
				if (num2 == 0)
				{
					var rec2 = new Rectangle((int)byteStringPointF2.X, (int)byteStringPointF2.Y, (int)(byteStringPointF3.X - byteStringPointF2.X + _charSize.Width), (int)_charSize.Height);
					if (rec2.IntersectsWith(_recStringView))
					{
						rec2.Intersect(_recStringView);
						PaintCurrentByteSign(g, rec2);
					}
					return;
				}
				var rec3 = new Rectangle((int)byteStringPointF2.X, (int)byteStringPointF2.Y, (int)(_recStringView.X + num - byteStringPointF2.X + _charSize.Width), (int)_charSize.Height);
				if (rec3.IntersectsWith(_recStringView))
				{
					rec3.Intersect(_recStringView);
					PaintCurrentByteSign(g, rec3);
				}
				if (num2 > 1)
				{
					var rec4 = new Rectangle(_recStringView.X, (int)(byteStringPointF2.Y + _charSize.Height), _recStringView.Width, (int)(_charSize.Height * (num2 - 1)));
					if (rec4.IntersectsWith(_recStringView))
					{
						rec4.Intersect(_recStringView);
						PaintCurrentByteSign(g, rec4);
					}
				}
				var rec5 = new Rectangle(_recStringView.X, (int)byteStringPointF3.Y, (int)(byteStringPointF3.X - _recStringView.X + _charSize.Width), (int)_charSize.Height);
				if (rec5.IntersectsWith(_recStringView))
				{
					rec5.Intersect(_recStringView);
					PaintCurrentByteSign(g, rec5);
				}
				return;
			}
			if (_selectionLength == 0)
			{
				var gridBytePoint4 = GetGridBytePoint(_bytePos - _startByte);
				var bytePointF = GetBytePointF(gridBytePoint4);
				var size2 = new Size((int)_charSize.Width * 2, (int)_charSize.Height);
				var rec6 = new Rectangle((int)bytePointF.X, (int)bytePointF.Y, size2.Width, size2.Height);
				PaintCurrentByteSign(g, rec6);
				return;
			}
			var num3 = (int)(_recHex.Width - _charSize.Width * 5f);
			var gridBytePoint5 = GetGridBytePoint(_bytePos - _startByte);
			var bytePointF2 = GetBytePointF(gridBytePoint5);
			var gridBytePoint6 = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1);
			var bytePointF3 = GetBytePointF(gridBytePoint6);
			var num4 = gridBytePoint6.Y - gridBytePoint5.Y;
			if (num4 == 0)
			{
				var rec7 = new Rectangle((int)bytePointF2.X, (int)bytePointF2.Y, (int)(bytePointF3.X - bytePointF2.X + _charSize.Width * 2f), (int)_charSize.Height);
				if (rec7.IntersectsWith(_recHex))
				{
					rec7.Intersect(_recHex);
					PaintCurrentByteSign(g, rec7);
				}
				return;
			}
			var rec8 = new Rectangle((int)bytePointF2.X, (int)bytePointF2.Y, (int)(_recHex.X + num3 - bytePointF2.X + _charSize.Width * 2f), (int)_charSize.Height);
			if (rec8.IntersectsWith(_recHex))
			{
				rec8.Intersect(_recHex);
				PaintCurrentByteSign(g, rec8);
			}
			if (num4 > 1)
			{
				var rec9 = new Rectangle(_recHex.X, (int)(bytePointF2.Y + _charSize.Height), (int)(num3 + _charSize.Width * 2f), (int)(_charSize.Height * (num4 - 1)));
				if (rec9.IntersectsWith(_recHex))
				{
					rec9.Intersect(_recHex);
					PaintCurrentByteSign(g, rec9);
				}
			}
			var rec10 = new Rectangle(_recHex.X, (int)bytePointF3.Y, (int)(bytePointF3.X - _recHex.X + _charSize.Width * 2f), (int)_charSize.Height);
			if (rec10.IntersectsWith(_recHex))
			{
				rec10.Intersect(_recHex);
				PaintCurrentByteSign(g, rec10);
			}
		}

		private void PaintCurrentByteSign(Graphics g, Rectangle rec)
		{
			if (rec.Top >= 0 && rec.Left >= 0 && rec.Width > 0 && rec.Height > 0)
			{
				var image = new Bitmap(rec.Width, rec.Height);
				var graphics = Graphics.FromImage(image);
				var brush = new SolidBrush(_shadowSelectionColor);
				graphics.FillRectangle(brush, 0, 0, rec.Width, rec.Height);
				g.CompositingQuality = CompositingQuality.GammaCorrected;
				g.DrawImage(image, rec.Left, rec.Top);
			}
		}

		private Color GetDefaultForeColor()
		{
			if (Enabled)
			{
				return ForeColor;
			}
			return Color.Gray;
		}

		private void UpdateVisibilityBytes()
		{
			if (_byteProvider != null && _byteProvider.Length != 0)
			{
				_startByte = (_scrollVpos + 1) * HorizontalByteCount - HorizontalByteCount;
				_endByte = Math.Min(_byteProvider.Length - 1, _startByte + _iHexMaxBytes);
			}
		}

		private void UpdateRectanglePositioning()
		{
			var sizeF = CreateGraphics().MeasureString("A", Font, 100, _stringFormat);
			_charSize = new SizeF((float)Math.Ceiling(sizeF.Width), (float)Math.Ceiling(sizeF.Height));
			_recContent = ClientRectangle;
			_recContent.X += _recBorderLeft;
			_recContent.Y += _recBorderTop;
			_recContent.Width -= _recBorderRight + _recBorderLeft;
			_recContent.Height -= _recBorderBottom + _recBorderTop;
			if (_vScrollBarVisible)
			{
				_recContent.Width -= _vScrollBar.Width;
				_vScrollBar.Left = _recContent.X + _recContent.Width;
				_vScrollBar.Top = _recContent.Y;
				_vScrollBar.Height = _recContent.Height;
			}
			var num = 4;
			if (_lineInfoVisible)
			{
				_recLineInfo = new Rectangle(_recContent.X + num, _recContent.Y, (int)(_charSize.Width * (LineInfoDigits + 2)), _recContent.Height);
			}
			else
			{
				_recLineInfo = Rectangle.Empty;
				_recLineInfo.X = num;
			}
			_recHex = new Rectangle(_recLineInfo.X + _recLineInfo.Width, _recLineInfo.Y, _recContent.Width - _recLineInfo.Width, _recContent.Height);
			if (UseFixedBytesPerLine)
			{
				SetHorizontalByteCount(_bytesPerLine);
				_recHex.Width = (int)Math.Floor(HorizontalByteCount * (double)_charSize.Width * 3.0 + 2f * _charSize.Width);
			}
			else
			{
				var num2 = (int)Math.Floor(_recHex.Width / (double)_charSize.Width);
				if (num2 > 1)
				{
					SetHorizontalByteCount((int)Math.Floor(num2 / 3.0));
				}
				else
				{
					SetHorizontalByteCount(num2);
				}
			}
			if (_stringViewVisible)
			{
				_recStringView = new Rectangle(_recHex.X + _recHex.Width, _recHex.Y, (int)(_charSize.Width * HorizontalByteCount), _recHex.Height);
			}
			else
			{
				_recStringView = Rectangle.Empty;
			}
			var verticalByteCount = (int)Math.Floor(_recHex.Height / (double)_charSize.Height);
			SetVerticalByteCount(verticalByteCount);
			_iHexMaxBytes = HorizontalByteCount * _iHexMaxVBytes;
			UpdateScrollSize();
		}

		private PointF GetBytePointF(long byteIndex)
		{
			var gridBytePoint = GetGridBytePoint(byteIndex);
			return GetBytePointF(gridBytePoint);
		}

		private PointF GetBytePointF(Point gp)
		{
			var num = 3f * _charSize.Width * gp.X + _recHex.X;
			var num2 = (gp.Y + 1) * _charSize.Height - _charSize.Height + _recHex.Y;
			return new PointF(num, num2);
		}

		private PointF GetByteStringPointF(Point gp)
		{
			var num = _charSize.Width * gp.X + _recStringView.X;
			var num2 = (gp.Y + 1) * _charSize.Height - _charSize.Height + _recStringView.Y;
			return new PointF(num, num2);
		}

		private Point GetGridBytePoint(long byteIndex)
		{
			var num = (int)Math.Floor(byteIndex / (double)HorizontalByteCount);
			var num2 = (int)(byteIndex + HorizontalByteCount - HorizontalByteCount * (num + 1));
			return new Point(num2, num);
		}

		private string ConvertBytesToHex(byte[] data)
		{
			var stringBuilder = new StringBuilder();
			foreach (var b in data)
			{
				var value = ConvertByteToHex(b);
				stringBuilder.Append(value);
				stringBuilder.Append(" ");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		private string ConvertByteToHex(byte b)
		{
			var text = b.ToString(_hexStringFormat, Thread.CurrentThread.CurrentCulture);
			if (text.Length == 1)
			{
				text = "0" + text;
			}
			return text;
		}

		private byte[] ConvertHexToBytes(string hex)
		{
			if (string.IsNullOrEmpty(hex))
			{
				return null;
			}
			hex = hex.Trim();
			var array = hex.Split(' ');
			var array2 = new byte[array.Length];
			for (var i = 0; i < array.Length; i++)
			{
				var hex2 = array[i];
				if (!ConvertHexToByte(hex2, out var b))
				{
					return null;
				}
				array2[i] = b;
			}
			return array2;
		}

		private bool ConvertHexToByte(string hex, out byte b)
		{
			return byte.TryParse(hex, NumberStyles.HexNumber, Thread.CurrentThread.CurrentCulture, out b);
		}

		private void SetPosition(long bytePos)
		{
			SetPosition(bytePos, _byteCharacterPos);
		}

		public void SetPosition(long bytePos, int byteCharacterPos)
		{
			if (_byteCharacterPos != byteCharacterPos)
			{
				_byteCharacterPos = byteCharacterPos;
			}
			if (bytePos != _bytePos)
			{
				_bytePos = bytePos;
				CheckCurrentLineChanged();
				CheckCurrentPositionInLineChanged();
				OnSelectionStartChanged(EventArgs.Empty);
			}
		}

		private void SetSelectionLength(long selectionLength)
		{
			if (selectionLength != _selectionLength)
			{
				_selectionLength = selectionLength;
				OnSelectionLengthChanged(EventArgs.Empty);
			}
		}

		private void SetHorizontalByteCount(int value)
		{
			if (HorizontalByteCount != value)
			{
				HorizontalByteCount = value;
				OnHorizontalByteCountChanged(EventArgs.Empty);
			}
		}

		private void SetVerticalByteCount(int value)
		{
			if (_iHexMaxVBytes != value)
			{
				_iHexMaxVBytes = value;
				OnVerticalByteCountChanged(EventArgs.Empty);
			}
		}

		private void CheckCurrentLineChanged()
		{
			var num = (long)Math.Floor(_bytePos / (double)HorizontalByteCount) + 1;
			if (_byteProvider == null && CurrentLine != 0)
			{
				CurrentLine = 0L;
				OnCurrentLineChanged(EventArgs.Empty);
			}
			else if (num != CurrentLine)
			{
				CurrentLine = num;
				OnCurrentLineChanged(EventArgs.Empty);
			}
		}

		private void CheckCurrentPositionInLineChanged()
		{
			var num = GetGridBytePoint(_bytePos).X + 1;
			if (_byteProvider == null && _currentPositionInLine != 0)
			{
				_currentPositionInLine = 0;
				OnCurrentPositionInLineChanged(EventArgs.Empty);
			}
			else if (num != _currentPositionInLine)
			{
				_currentPositionInLine = num;
				OnCurrentPositionInLineChanged(EventArgs.Empty);
			}
		}

		protected virtual void OnInsertActiveChanged(EventArgs e)
		{
            InsertActiveChanged?.Invoke(this, e);
        }

		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
            ReadOnlyChanged?.Invoke(this, e);
        }

		protected virtual void OnByteProviderChanged(EventArgs e)
		{
            ByteProviderChanged?.Invoke(this, e);
        }

		protected virtual void OnSelectionStartChanged(EventArgs e)
		{
            SelectionStartChanged?.Invoke(this, e);
        }

		protected virtual void OnSelectionLengthChanged(EventArgs e)
		{
            SelectionLengthChanged?.Invoke(this, e);
        }

		protected virtual void OnLineInfoVisibleChanged(EventArgs e)
		{
            LineInfoVisibleChanged?.Invoke(this, e);
        }

		protected virtual void OnStringViewVisibleChanged(EventArgs e)
		{
            StringViewVisibleChanged?.Invoke(this, e);
        }

		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
            BorderStyleChanged?.Invoke(this, e);
        }

		protected virtual void OnUseFixedBytesPerLineChanged(EventArgs e)
		{
            UseFixedBytesPerLineChanged?.Invoke(this, e);
        }

		protected virtual void OnBytesPerLineChanged(EventArgs e)
		{
            BytesPerLineChanged?.Invoke(this, e);
        }

		protected virtual void OnVScrollBarVisibleChanged(EventArgs e)
		{
            VScrollBarVisibleChanged?.Invoke(this, e);
        }

		protected virtual void OnHexCasingChanged(EventArgs e)
		{
            HexCasingChanged?.Invoke(this, e);
        }

		protected virtual void OnHorizontalByteCountChanged(EventArgs e)
		{
            HorizontalByteCountChanged?.Invoke(this, e);
        }

		protected virtual void OnVerticalByteCountChanged(EventArgs e)
		{
            VerticalByteCountChanged?.Invoke(this, e);
        }

		protected virtual void OnCurrentLineChanged(EventArgs e)
		{
            CurrentLineChanged?.Invoke(this, e);
        }

		protected virtual void OnCurrentPositionInLineChanged(EventArgs e)
		{
            CurrentPositionInLineChanged?.Invoke(this, e);
        }

		protected virtual void OnCopied(EventArgs e)
		{
            Copied?.Invoke(this, e);
        }

		protected virtual void OnCopiedHex(EventArgs e)
		{
            CopiedHex?.Invoke(this, e);
        }

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!Focused)
			{
				Focus();
			}
			if (e.Button == MouseButtons.Left)
			{
				SetCaretPosition(new Point(e.X, e.Y));
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var lines = -(e.Delta * SystemInformation.MouseWheelScrollLines / 120);
			PerformScrollLines(lines);
			base.OnMouseWheel(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateRectanglePositioning();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			CreateCaret();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			DestroyCaret();
		}

		private void _byteProvider_LengthChanged(object sender, EventArgs e)
		{
			UpdateScrollSize();
		}
	}
}
