#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Janela para marcar objetos na Hierarchy com cores personalizadas
/// </summary>
public class MarkerWindow : EditorWindow
{
    private static Dictionary<string, Color> markers = new Dictionary<string, Color>();
    private static List<Color> recentColors = new List<Color>();
    private Color selectedColor = Color.green;
    private const int maxRecentColors = 8;

    [MenuItem("Window/Color Markers")]
    public static void ShowWindow()
    {
        GetWindow<MarkerWindow>("Marcadores");
        LoadMarkers();
        LoadRecentColors();
    }

    private void OnGUI()
    {
        if (Selection.activeGameObject != null)
        {
            GUILayout.Label("Objeto Selecionado:", EditorStyles.boldLabel);
            GUILayout.Label(Selection.activeGameObject.name);

            selectedColor = EditorGUILayout.ColorField("Cor do Marcador", selectedColor);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Salvar Cor"))
            {
                string path = GetGameObjectPath(Selection.activeGameObject);
                markers[path] = selectedColor;
                SaveMarkers(); // Salva os marcadores
                AddRecentColor(selectedColor); // Salva a cor nos recentes
                SaveRecentColors();            // Persiste as cores recentes
                EditorApplication.RepaintHierarchyWindow();
            }

            if (GUILayout.Button("Limpar Cor"))
            {
                string path = GetGameObjectPath(Selection.activeGameObject);
                if (markers.ContainsKey(path))
                {
                    markers.Remove(path);
                    EditorPrefs.DeleteKey("Marker_" + path);
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("Cores Recentes:", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            foreach (Color c in recentColors)
            {
                GUI.backgroundColor = c;
                if (GUILayout.Button("", GUILayout.Width(30), GUILayout.Height(20)))
                {
                    selectedColor = c;
                    GUI.FocusControl(null); // Tira foco para aplicar a cor visualmente
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("Nenhum objeto selecionado.");
        }
    }

    // Inicializa no load do editor
    [InitializeOnLoadMethod]
    private static void Init()
    {
        LoadMarkers(); // Carrega os marcadores ao iniciar o editor
        LoadRecentColors(); // Carrega as cores recentes
        ApplyMarkersToHierarchy(); // Aplica as cores automaticamente

        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

        // Força a atualização da Hierarchy ao abrir o Unity
        EditorApplication.delayCall += () =>
        {
            EditorApplication.RepaintHierarchyWindow();
        };
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        string path = GetGameObjectPath(obj);

        if (markers.TryGetValue(path, out Color color))
        {
            // Expande até ocupar toda a largura da Hierarchy
            selectionRect.x = 0;
            selectionRect.width = selectionRect.xMax + 1000f; // Força ir até o final

            // Fundo semi-transparente
            Color bg = color;
            bg.a = 0.25f;
            EditorGUI.DrawRect(selectionRect, bg);
        }
    }

    private static string GetGameObjectPath(GameObject obj)
    {
        return obj.transform.parent == null
            ? obj.name
            : GetGameObjectPath(obj.transform.parent.gameObject) + "/" + obj.name;
    }

    // Persistência dos marcadores
    private static void SaveMarkers()
    {
        foreach (var kv in markers)
        {
            string colorString = ColorUtility.ToHtmlStringRGBA(kv.Value);
            EditorPrefs.SetString("Marker_" + kv.Key, colorString);
        }
    }

    private static void LoadMarkers()
    {
        markers.Clear();
        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            string path = GetGameObjectPath(obj);
            string key = "Marker_" + path;

            if (EditorPrefs.HasKey(key))
            {
                string colorString = EditorPrefs.GetString(key);
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                {
                    markers[path] = c;
                }
            }
        }
    }

    // Aplica as cores dos marcadores aos objetos na Hierarchy
    private static void ApplyMarkersToHierarchy()
    {
        foreach (var kv in markers)
        {
            string path = kv.Key;
            Color color = kv.Value;

            // Buscar o GameObject correspondente pelo path
            GameObject obj = FindGameObjectByPath(path);
            if (obj != null)
            {
                // Aqui, você pode adicionar a lógica de como a cor é aplicada, 
                // por exemplo, no material do objeto, ou no fundo da Hierarchy, como já feito.
                // No caso, já estamos desenhando a cor no fundo da Hierarchy na função OnHierarchyGUI
            }
        }
    }

    // Busca o GameObject com base no caminho completo
    private static GameObject FindGameObjectByPath(string path)
    {
        string[] pathParts = path.Split('/');
        GameObject obj = null;

        foreach (string part in pathParts)
        {
            if (obj == null)
            {
                obj = GameObject.Find(part);
            }
            else
            {
                obj = FindChildByName(obj.transform, part);
            }

            if (obj == null)
                return null;
        }

        return obj;
    }

    // Encontra o filho pelo nome dentro de um transform
    private static GameObject FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;
        }

        return null;
    }

    // Adiciona a cor à lista de recentes
    private static void AddRecentColor(Color color)
    {
        // Remove se já existir (para reordenar)
        recentColors.RemoveAll(c => ApproximatelyEqual(c, color));

        // Adiciona no início
        recentColors.Insert(0, color);

        // Limita a quantidade de cores recentes
        if (recentColors.Count > maxRecentColors)
            recentColors.RemoveAt(recentColors.Count - 1);
    }

    // Verifica se duas cores são aproximadamente iguais (para evitar duplicação)
    private static bool ApproximatelyEqual(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b) &&
               Mathf.Approximately(a.a, b.a);
    }

    // Persiste as cores recentes no EditorPrefs
    private static void SaveRecentColors()
    {
        for (int i = 0; i < recentColors.Count; i++)
        {
            string colorString = ColorUtility.ToHtmlStringRGBA(recentColors[i]);
            EditorPrefs.SetString("RecentColor_" + i, colorString);
        }
        EditorPrefs.SetInt("RecentColorCount", recentColors.Count); // Grava a quantidade de cores recentes
    }

    // Carrega as cores recentes do EditorPrefs
    private static void LoadRecentColors()
    {
        recentColors.Clear();
        int count = EditorPrefs.GetInt("RecentColorCount", 0); // Pega a quantidade de cores
        for (int i = 0; i < count; i++)
        {
            string key = "RecentColor_" + i;
            if (EditorPrefs.HasKey(key))
            {
                string colorString = EditorPrefs.GetString(key);
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color c))
                {
                    recentColors.Add(c);
                }
            }
        }
    }
}
#endif