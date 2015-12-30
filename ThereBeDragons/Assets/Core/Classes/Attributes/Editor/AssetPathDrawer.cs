
namespace Nucleus.Attributes.Internal
{
  using UnityEngine;
  using UnityEditor;
  using System;
  using System.IO;
  using System.Collections.Generic;
  using UObject = UnityEngine.Object;

  [CustomPropertyDrawer( typeof( AssetPathAttribute ) )]
  internal class AssetPathDrawer : PropertyDrawer
  {
    private AssetPathAttribute _attribute;
    private string _objectPath;
    private string _objectGUID;
    private Dictionary<string, UObject> _objDictionary = new Dictionary<string, UObject>();
    private const string RESOURCE_FOLDER_NAME = "Resources/";
    private const string EDITOR_PREF_KEY = "AssetPathDrawer Path2GUID KEY:";

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
      _attribute = attribute as AssetPathAttribute;

      if(property.propertyType == SerializedPropertyType.String)
      {
        DrawString(position, property, label);
      }
      else
      {
        GUI.Label(position, "This only works with strings.");
      }
    }

    private void DrawString(Rect position, SerializedProperty property, GUIContent label)
    {
      _objectPath = property.stringValue;

      UObject obj = null;

      if( !string.IsNullOrEmpty( _objectPath ) )
      {
        if( _objDictionary.ContainsKey( _objectPath ) )
        {
          obj = _objDictionary[_objectPath];
        }
        else
        {
          _objDictionary.Add( _objectPath, null );
        }
      }
      
      if (property.propertyType == SerializedPropertyType.String)
      {
        if( obj == null && _objectPath != string.Empty )
        {
          if( _attribute.requiredInResourcesFolder )
          {
            obj = Resources.Load( _objectPath, _attribute.assetType );
          }
          else
          {
            //Scriptable Objects can't be converted :(
            if( _attribute.assetType == typeof( ScriptableObject ) )
            {
              obj = AssetDatabase.LoadAssetAtPath( _objectPath, typeof( ScriptableObject ) );
            }
            else
            {

              obj = (UnityEngine.Object)Convert.ChangeType( AssetDatabase.LoadAssetAtPath( _objectPath, _attribute.assetType ), _attribute.assetType );
            }
          }

          //Asset Database Lookup
          if( obj == null )
          {
            _objectGUID = EditorPrefs.GetString(EDITOR_PREF_KEY + _objectPath, string.Empty);
            
            string[] results = AssetDatabase.FindAssets( "t:" + _attribute.assetType.Name + " " + Path.GetFileName( _objectPath ) );

            if( results.Length > 0 )
            {
              
              
              string path = AssetDatabase.GUIDToAssetPath( results[0] );


              if( _attribute.requiredInResourcesFolder )
              {
                int index = path.LastIndexOf( RESOURCE_FOLDER_NAME ) + RESOURCE_FOLDER_NAME.Length;
                if( index != -1 )
                {
                  _objectPath = path.Substring( index, path.Length - index - Path.GetExtension( path ).Length );

                  obj = Resources.Load( _objectPath );
                }
              }
              else
              {
                property.stringValue = path;

                _objectPath = path;

                obj = AssetDatabase.LoadAssetAtPath( path, _attribute.assetType );
              }
            }
          }

          //GUID Lookup
          if( obj == null )
          {
            _objectGUID = EditorPrefs.GetString( EDITOR_PREF_KEY + _objectPath, string.Empty );

            if( !string.IsNullOrEmpty( _objectGUID ) )
            {
              _objectPath = AssetDatabase.GUIDToAssetPath( _objectGUID );

              obj = AssetDatabase.LoadAssetAtPath( _objectPath, _attribute.assetType );

              if( obj == null )
              {
                _objectPath = null;
                Debug.LogWarning( "AssetPath Attribute Warning, Resource has be removed or renamed. Please add it again." );
              }
            }
            else
            {
              _objectPath = null;
              Debug.LogWarning( "AssetPath Attribute Warning, Resource has be removed or renamed. Please add it again." );
            }
            
          }
        }
        
        EditorGUI.BeginChangeCheck();
        {
          obj = EditorGUI.ObjectField( position, label, obj, _attribute.assetType, false );
        }
        if( EditorGUI.EndChangeCheck() )
        {
          if(obj != null)
          {
            OnAssetChanged(property, ref obj);
          }
          else
          {
            obj = null;
            _objectPath = string.Empty;
          }
        }
      }
      else
      {
        GUI.Label( position, "Asset Path Attribute only vaild for strings." );
      }

      property.stringValue = _objectPath;
      if(_objectPath != null)
      {
        _objDictionary[_objectPath] = obj;
      }
    }

    private void OnAssetChanged(SerializedProperty property, ref UObject obj)
    {
      _objectPath = AssetDatabase.GetAssetPath(obj);
      _objectGUID = AssetDatabase.AssetPathToGUID(_objectPath);

      if (_attribute.requiredInResourcesFolder)
      {
        if (_objectPath.Contains(RESOURCE_FOLDER_NAME))
        {
          //Now we need to strip the asset of the resource path
          int startIndex = _objectPath.IndexOf(RESOURCE_FOLDER_NAME);
          int resourcePathLength = startIndex + RESOURCE_FOLDER_NAME.Length;
          _objectPath = _objectPath.Substring(resourcePathLength, _objectPath.Length - resourcePathLength);

          _objectPath = _objectPath.Substring(0, _objectPath.IndexOf('.'));

          //Save the path to EditorPrefs so we can try to get it if it moves.
          EditorPrefs.SetString(EDITOR_PREF_KEY + _objectPath, _objectGUID);

          property.stringValue = _objectPath;
        }
        else
        {
          //The have placed a non resource asset in their asset folder.
          Debug.LogError(string.Format("AssetPathAttribute Error! \"{0}\" is not located in the required Resource folder. It will not be added.", obj.name));
          obj = null;
          _objectPath = string.Empty;
        }
      }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return base.GetPropertyHeight(property, label);
    }
  }
}
