using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
namespace SoundEventLink.Editor
{
	public class DuckingVisualizeTreeViewItem : TreeViewItem
	{
		public string Key { get; set; }
		public float InWeight { get; set; }
		public float OutWeight { get; set; }
		public float Volume { get; set; }
		public int Priority { get; set; }
		
		public DuckingVisualizeTreeViewItem(int i)
			: base(i)
		{
			
		}
	}
	
	public class DuckingVisualizeTreeView : TreeView
	{
		private const string SortedColumnIndexStateKey = "DuckingVisualizeTreeView_sortedColumnIndex";

		public IReadOnlyList<TreeViewItem> CurrentBindingItems;

		public DuckingVisualizeTreeView()
			: this(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
			{
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Key"),
					width         = 20
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("InWeight"),
					width         = 10
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("OutWeight"),
					width         = 10
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Volume"),
					width         = 10
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Priority")
				}
			})))
		{
			
		}
		private DuckingVisualizeTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) 
			: base(state, multiColumnHeader)
		{
			rowHeight                     = 20;
			showAlternatingRowBackgrounds = true;
			showBorder                    = true;

			multiColumnHeader.sortingChanged += HeaderSortingChanged;
			multiColumnHeader.ResizeToFit();
			Reload();

			multiColumnHeader.sortedColumnIndex = SessionState.GetInt(SortedColumnIndexStateKey, 1);
		}
		
		public void ReloadAndSort()
		{
			var currentSelected = state.selectedIDs;
			Reload();
			HeaderSortingChanged(multiColumnHeader);
			state.selectedIDs = currentSelected;
		}

		private void HeaderSortingChanged(MultiColumnHeader header)
		{
			SessionState.SetInt(SortedColumnIndexStateKey, header.sortedColumnIndex);
			var index     = header.sortedColumnIndex;
			var ascending = header.IsSortedAscending(header.sortedColumnIndex);

			var items = rootItem.children.Cast<DuckingVisualizeTreeViewItem>();

			var orderedEnumerable = index switch
			{
				0 => @ascending ? items.OrderBy(item => item.Key) : items.OrderByDescending(item => item.Key),
				1 => @ascending ? items.OrderBy(item => item.InWeight) : items.OrderByDescending(item => item.InWeight),
				2 => @ascending ? items.OrderBy(item => item.OutWeight) : items.OrderByDescending(item => item.OutWeight),
				3 => @ascending ? items.OrderBy(item => item.Volume) : items.OrderByDescending(item => item.Volume),
				4 => @ascending ? items.OrderBy(item => item.Priority) : items.OrderByDescending(item => item.Priority),
				_ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
			};

			CurrentBindingItems = rootItem.children = orderedEnumerable.Cast<TreeViewItem>().ToList();
			BuildRows(rootItem);
		}
		
		protected override TreeViewItem BuildRoot()
		{
			var root     = new TreeViewItem { depth = -1 };
			var children = new List<TreeViewItem>();

			if (Runtime.SoundEventLink.Instance != null && 
				Runtime.SoundEventLink.Instance.DuckingController != null)
			{
				var i = 0;
				Runtime.SoundEventLink.Instance.DuckingController
				       .ForEachActiveDucking((key, inWeight, outWeight, volume, priority) => 
				       {
					       children.Add(new DuckingVisualizeTreeViewItem(i)
					       {
						       Key = key,
						       InWeight = inWeight,
						       OutWeight = outWeight,
						       Volume = volume,
						       Priority = priority
					       });
					       i++;
				       });
			}

			CurrentBindingItems = children;
			root.children       = CurrentBindingItems as List<TreeViewItem>;

			return root;
		}
		protected override bool CanMultiSelect(TreeViewItem item) => false;

		protected override void RowGUI(RowGUIArgs args)
		{
			var item = args.item as DuckingVisualizeTreeViewItem;

			Debug.Assert(item != null, nameof(item) + " != null");
			
			for (var i = 0; i < args.GetNumVisibleColumns(); i++)
			{
				var rect      = args.GetCellRect(i);
				var columnIdx = args.GetColumn(i);

				var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;
				labelStyle.alignment = TextAnchor.MiddleLeft;
				switch (columnIdx)
				{
					case 0:
						EditorGUI.LabelField(rect, item.Key, labelStyle);
						break;
					case 1:
						EditorGUI.LabelField(rect, item.InWeight.ToString(), labelStyle);
						break;
					case 2:
						EditorGUI.LabelField(rect, item.OutWeight.ToString(), labelStyle);
						break;
					case 3:
						EditorGUI.LabelField(rect, item.Volume.ToString(), labelStyle);
						break;
					case 4:
						EditorGUI.LabelField(rect, item.Priority.ToString(), labelStyle);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(columnIdx), columnIdx, null);
				}
			}
		}
	}

}