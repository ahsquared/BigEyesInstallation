using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Linq;

using VacuumShaders;
 
 
public class VertexColor_MaterialEditor : MaterialEditor
{
    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Variables                                                                 //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////
    MaterialProperty _Color;
    MaterialProperty _MainTex;

    MaterialProperty _ReflectColor;
    MaterialProperty _Cube;

    MaterialProperty _V_VC_IBL_Cube;
    MaterialProperty _V_VC_IBL_Cube_Intensity;
    MaterialProperty _V_VC_IBL_Cube_Contrast;
    MaterialProperty _V_VC_IBL_Light_Intensity;

    MaterialProperty _V_VC_EmissionColor;
    MaterialProperty _V_VC_EmissionStrength;


    Material targetMaterial;
    Shader currentShader;
    string[] keyWords;

    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Unity Functions                                                           //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////
    public override void OnEnable()
    {
        base.OnEnable();

        targetMaterial = target as Material;
        currentShader = targetMaterial.shader;

        LoadParameters();

    }

    public override void OnInspectorGUI()
    {
        //Check VacuumShaders folder in the main Assets folder
        if (VacuumEditorUtility.IsDllReady() == false)
        {
            EditorGUILayout.HelpBox("VacuumShaders folder must be under main Assets folder.", MessageType.Error);
            return;
        }

        base.OnInspectorGUI();

        if (isVisible == false || targets.Length > 1 || targetMaterial == null)
            return;

        if (currentShader != targetMaterial.shader)
        {
            OnEnable();
            Repaint();
        }
        if (string.IsNullOrEmpty(targetMaterial.GetTag("VertexColor", false)))
            return;

        keyWords = targetMaterial.shaderKeywords;


        Draw_MainColors();
        Draw_Reflection();
        Draw_IBL();
        Draw_Emission();
    }

    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Custom Functions                                                          //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////
    void LoadParameters()
    {
        Material[] mats = new Material[] { targetMaterial };

        _Color = GetMaterialProperty(mats, "_Color");
        _MainTex = GetMaterialProperty(mats, "_MainTex");

        _ReflectColor = GetMaterialProperty(mats, "_ReflectColor");
        _Cube = GetMaterialProperty(mats, "_Cube");

        _V_VC_IBL_Cube = GetMaterialProperty(mats, "_V_VC_IBL_Cube");
        _V_VC_IBL_Cube_Intensity = GetMaterialProperty(mats, "_V_VC_IBL_Cube_Intensity");
        _V_VC_IBL_Cube_Contrast = GetMaterialProperty(mats, "_V_VC_IBL_Cube_Contrast");
        _V_VC_IBL_Light_Intensity = GetMaterialProperty(mats, "_V_VC_IBL_Light_Intensity");

        _V_VC_EmissionColor = GetMaterialProperty(mats, "_V_VC_EmissionColor");
        _V_VC_EmissionStrength = GetMaterialProperty(mats, "_V_VC_EmissionStrength");
    }

    void ForceLoadParameters(ref MaterialProperty _prop, string _propName)
    {
        Material[] mats = new Material[] { targetMaterial };

        _prop = GetMaterialProperty(mats, _propName);
    }

    public void ResetMaterial()
    {
        targetMaterial = target as Material;
        if (targetMaterial == null)
            return;

        LoadParameters();
        keyWords = targetMaterial.shaderKeywords;


        ModifyKeyWords(false, "V_VC_MAIN_COLORS_ON", "V_VC_MAIN_COLORS_OFF");
        _Color.colorValue = Color.white;
        _MainTex.textureValue = null;


        ModifyKeyWords(false, "V_VC_REFLECTION_ON", "V_VC_REFLECTION_OFF");
        _ReflectColor.colorValue = Color.white;
        _Cube.textureValue = null;

        ModifyKeyWords(false, "V_VC_IBL_ON", "V_VC_IBL_OFF");
        _V_VC_IBL_Cube.textureValue = null;
        _V_VC_IBL_Cube_Intensity.floatValue = 1;
        _V_VC_IBL_Cube_Contrast.floatValue = 1;
        _V_VC_IBL_Light_Intensity.floatValue = 0f;

        ModifyKeyWords(false, "V_VC_EMISSION_ON", "V_VC_EMISSION_OFF");
        _V_VC_EmissionColor.colorValue = Color.white;
        _V_VC_EmissionStrength.floatValue = 1;

        Repaint();
    }

    void ModifyKeyWords(bool _value, string _enableStr, string _disableStr)
    {
        List<string> newKeywords = keyWords.ToList();
        newKeywords.Remove(_enableStr);
        newKeywords.Remove(_disableStr);

        if (_value)
            newKeywords.Add(_enableStr);

        Undo.RecordObject(targetMaterial, "Curved World - Toggle " + _enableStr);
        targetMaterial.shaderKeywords = newKeywords.ToArray();

        EditorUtility.SetDirty(targetMaterial);
    }


    void Draw_MainColors()
    {
        if (_Color == null)
            ForceLoadParameters(ref _Color, "_Color");
        if (_MainTex == null)
            ForceLoadParameters(ref _MainTex, "_MainTex");

        if (_Color == null || _MainTex == null)
            return;


        bool value = keyWords.Contains("V_VC_MAIN_COLORS_ON");
        EditorGUI.BeginChangeCheck();

        
        value = EditorGUILayout.ToggleLeft(" Base Color & Texture", value, EditorStyles.largeLabel);
        if (EditorGUI.EndChangeCheck())
        {
            ModifyKeyWords(value, "V_VC_MAIN_COLORS_ON", "V_VC_MAIN_COLORS_OFF");
        }

        if (value == true)
        {
            using (new VacuumEditorGUIUtility.EditorGUIIndentLevel(1))
            {
                GUILayout.Space(5);
                ColorProperty(_Color, "Tint Color");
                TextureProperty(_MainTex, "Base Texture");
            }

            GUILayout.Space(5);
        }
    }

    void Draw_Reflection()
    {
        if (_ReflectColor == null)
            ForceLoadParameters(ref _ReflectColor, "_ReflectColor");
        if (_Cube == null)
            ForceLoadParameters(ref _Cube, "_Cube");

        if (_ReflectColor == null || _Cube == null)
            return;


        bool value = keyWords.Contains("V_VC_REFLECTION_ON");
        EditorGUI.BeginChangeCheck();


        value = EditorGUILayout.ToggleLeft(" Reflection", value, EditorStyles.largeLabel);
        if (EditorGUI.EndChangeCheck())
        {
            ModifyKeyWords(value, "V_VC_REFLECTION_ON", "V_VC_REFLECTION_OFF");
        }

        if (value == true)
        {
            using (new VacuumEditorGUIUtility.EditorGUIIndentLevel(1))
            {
                GUILayout.Space(5);
                ColorProperty(_ReflectColor, "Reflection Color");
                TextureProperty(_Cube, "Cube", false);
            }

            GUILayout.Space(5);
        }
    }

    void Draw_IBL()
    {
        if (_V_VC_IBL_Cube == null)
            ForceLoadParameters(ref _V_VC_IBL_Cube, "_V_VC_IBL_Cube");
        if (_V_VC_IBL_Cube_Intensity == null)
            ForceLoadParameters(ref _V_VC_IBL_Cube_Intensity, "_V_VC_IBL_Cube_Intensity");
        if (_V_VC_IBL_Cube_Contrast == null)
            ForceLoadParameters(ref _V_VC_IBL_Cube_Contrast, "_V_VC_IBL_Cube_Contrast");
        if (_V_VC_IBL_Light_Intensity == null)
            ForceLoadParameters(ref _V_VC_IBL_Light_Intensity, "_V_VC_IBL_Light_Intensity");


        if (_V_VC_IBL_Cube == null || _V_VC_IBL_Cube_Intensity == null || _V_VC_IBL_Cube_Contrast == null || _V_VC_IBL_Light_Intensity == null)
            return ;


        bool value = keyWords.Contains("V_VC_IBL_ON");
        EditorGUI.BeginChangeCheck();

        
        value = EditorGUILayout.ToggleLeft(" Image Based Ligthing", value, EditorStyles.largeLabel);
        if (EditorGUI.EndChangeCheck())
        {
            ModifyKeyWords(value, "V_VC_IBL_ON", "V_VC_IBL_OFF");
        }

        if (value == true)
        {
            using (new VacuumEditorGUIUtility.EditorGUIIndentLevel(1))
            {
                GUILayout.Space(5);
                TextureProperty(_V_VC_IBL_Cube, "IBL Cube", false);
                FloatProperty(_V_VC_IBL_Cube_Intensity, "IBL Intensity");
                FloatProperty(_V_VC_IBL_Cube_Contrast, "IBL Contrast");
                 
                float iblLightIntensity = _V_VC_IBL_Light_Intensity.floatValue;
                EditorGUI.BeginChangeCheck();
                iblLightIntensity = EditorGUILayout.Slider("Light Intensity", iblLightIntensity, -1.0f, 1.0f);
                if (EditorGUI.EndChangeCheck())
                    _V_VC_IBL_Light_Intensity.floatValue = iblLightIntensity;
                //FloatProperty(_V_VC_IBL_Light_Intensity, "Light Intensity");
            }

            GUILayout.Space(5);
        }
    }

    void Draw_Emission()
    {
        if (_V_VC_EmissionColor == null)
            ForceLoadParameters(ref _V_VC_EmissionColor, "_V_VC_EmissionColor");
        if (_V_VC_EmissionStrength == null)
            ForceLoadParameters(ref _V_VC_EmissionStrength, "_V_VC_EmissionStrength");

        if (_V_VC_EmissionColor == null || _V_VC_EmissionStrength == null)
            return;


        bool value = keyWords.Contains("V_VC_EMISSION_ON");
        EditorGUI.BeginChangeCheck();

        
        value = EditorGUILayout.ToggleLeft(" Emission", value, EditorStyles.largeLabel);
        if (EditorGUI.EndChangeCheck())
        {
            ModifyKeyWords(value, "V_VC_EMISSION_ON", "V_VC_EMISSION_OFF");
        }

        if (value == true)
        {
            using (new VacuumEditorGUIUtility.EditorGUIIndentLevel(1))
            {
                GUILayout.Space(5);
                ColorProperty(_V_VC_EmissionColor, "Emission Color");
                FloatProperty(_V_VC_EmissionStrength, "Emission Strength");
            }

            GUILayout.Space(5);
        }
    }
}
 