using System.IO;
using UnityEditor;

namespace Example
{
	public partial class CustomMenu
	{
		private const string MenuTitle = "CustomMenu";

		const int SceneListPriority = 30;
		const int DefineSymbolPriority = 100;
		const int BuildClientPriority = 200;
		const int AssetBundlePriority = 201;
		const int SecureAssetBundlePriority = 202;


		[MenuItem(MenuTitle + "/Scene/Create Scene List", priority = SceneListPriority)]
		static void CreateSceneList()
		{
			SceneListCreator.Create("Example/Scenes", "Example/Editor", "CustomMenu", 0, "Example");
		}


		[InitializeOnLoad]
		private class Startup
		{
			const string StartupFile = "Temp/startup";

			static Startup()
			{
				// NOTE : 通常だとEditorビルド毎に呼ばれてしまう 初回起動時”だけ”に実行したいので起動時に適当なファイルを作りそれで判定
				if (File.Exists(StartupFile))
				{
					return;
				}
				File.Create(StartupFile);

				// NOTE : 起動直後だとまだLibraryの構成が完了していないので少し待ってから実行
				EditorApplication.delayCall += Initialize;
			}
			static void Initialize()
			{
				EditorApplication.delayCall -= Initialize;
				ApplyMenuChecked();
			}
		}

		private class ApplyChecked : UnityEditor.Build.IActiveBuildTargetChanged
		{
			public int callbackOrder { get { return 0; } }
			public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget) => ApplyMenuChecked();
		}

		static void ApplyMenuChecked()
		{
			ApplySymbolMenuChecked();
		}
	}
}
