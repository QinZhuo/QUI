Shader "QUI/QMaskUI"
{
    Properties
	{
		_StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        [NoScaleOffset]_Mask("Mask", 2D) = "white" {}
        _MaskTillingOffset("MaskTillingOffset", Vector) = (1, 1, 0, 0)
        _Reverse("Reverse", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
	{
		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}
		ColorMask[_ColorMask]
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Off
         Blend SrcAlpha OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.color = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _Mask_TexelSize;
        float4 _MaskTillingOffset;
        float _Reverse;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        TEXTURE2D(_Mask);
        SAMPLER(sampler_Mask);

            // Graph Functions
            
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Preview_float(float In, out float Out)
        {
            Out = In;
        }

        void Unity_Step_float2(float2 Edge, float2 In, out float2 Out)
        {
            Out = step(Edge, In);
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        struct Bindings_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d
        {
        };

        void SG_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d(float4 Vector4_19C7703B, UnityTexture2D Texture2D_CA72CD38, float2 Vector2_F19B6F36, Bindings_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d IN, out float4 Output_1, out float R_2, out float G_3, out float B_4, out float A_5)
        {
            UnityTexture2D _Property_c20210f75e5841c8b4b7250cc2e4321e_Out_0 = Texture2D_CA72CD38;
            float2 _Property_10f7dba9ddb54aba84adb08a27e022b0_Out_0 = Vector2_F19B6F36;
            float4 _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0 = Vector4_19C7703B;
            float _Split_117dfb2a277c4a979671228912a2d2f1_R_1 = _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0[0];
            float _Split_117dfb2a277c4a979671228912a2d2f1_G_2 = _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0[1];
            float _Split_117dfb2a277c4a979671228912a2d2f1_B_3 = _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0[2];
            float _Split_117dfb2a277c4a979671228912a2d2f1_A_4 = _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0[3];
            float2 _Vector2_9c22647769ea4c96af77199c7f700b00_Out_0 = float2(_Split_117dfb2a277c4a979671228912a2d2f1_R_1, _Split_117dfb2a277c4a979671228912a2d2f1_G_2);
            float2 _Vector2_729da08c140e4f0db6fd3c2399651a00_Out_0 = float2(_Split_117dfb2a277c4a979671228912a2d2f1_B_3, _Split_117dfb2a277c4a979671228912a2d2f1_A_4);
            float2 _TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3;
            Unity_TilingAndOffset_float(_Property_10f7dba9ddb54aba84adb08a27e022b0_Out_0, _Vector2_9c22647769ea4c96af77199c7f700b00_Out_0, _Vector2_729da08c140e4f0db6fd3c2399651a00_Out_0, _TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3);
            float4 _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c20210f75e5841c8b4b7250cc2e4321e_Out_0.tex, _Property_c20210f75e5841c8b4b7250cc2e4321e_Out_0.samplerstate, _TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3);
            float _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_R_4 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0.r;
            float _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_G_5 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0.g;
            float _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_B_6 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0.b;
            float _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_A_7 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0.a;
            float _Preview_49bd68e7bd714c78ab709ed19049b7ff_Out_1;
            Unity_Preview_float(_SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_A_7, _Preview_49bd68e7bd714c78ab709ed19049b7ff_Out_1);
            float2 _Step_74b80af085fb44dab793b8aebd02e674_Out_2;
            Unity_Step_float2(float2(0, 0), _TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3, _Step_74b80af085fb44dab793b8aebd02e674_Out_2);
            float2 _Step_cfc0107733af4fbab0e5b4ffd1a8c95b_Out_2;
            Unity_Step_float2(_TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3, float2(1, 1), _Step_cfc0107733af4fbab0e5b4ffd1a8c95b_Out_2);
            float2 _Multiply_daacb5e105374afab1c4f7ac68f4c3ae_Out_2;
            Unity_Multiply_float(_Step_74b80af085fb44dab793b8aebd02e674_Out_2, _Step_cfc0107733af4fbab0e5b4ffd1a8c95b_Out_2, _Multiply_daacb5e105374afab1c4f7ac68f4c3ae_Out_2);
            float _Split_41b59f89add144378ab7e9985e715d68_R_1 = _Multiply_daacb5e105374afab1c4f7ac68f4c3ae_Out_2[0];
            float _Split_41b59f89add144378ab7e9985e715d68_G_2 = _Multiply_daacb5e105374afab1c4f7ac68f4c3ae_Out_2[1];
            float _Split_41b59f89add144378ab7e9985e715d68_B_3 = 0;
            float _Split_41b59f89add144378ab7e9985e715d68_A_4 = 0;
            float _Multiply_4a5f7cee40424bd4968573d70b63d22d_Out_2;
            Unity_Multiply_float(_Preview_49bd68e7bd714c78ab709ed19049b7ff_Out_1, _Split_41b59f89add144378ab7e9985e715d68_R_1, _Multiply_4a5f7cee40424bd4968573d70b63d22d_Out_2);
            float _Multiply_9c1d6ef3beeb45f9ae8fc5f2c61ead2a_Out_2;
            Unity_Multiply_float(_Multiply_4a5f7cee40424bd4968573d70b63d22d_Out_2, _Split_41b59f89add144378ab7e9985e715d68_G_2, _Multiply_9c1d6ef3beeb45f9ae8fc5f2c61ead2a_Out_2);
            Output_1 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0;
            R_2 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_R_4;
            G_3 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_G_5;
            B_4 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_B_6;
            A_5 = _Multiply_9c1d6ef3beeb45f9ae8fc5f2c61ead2a_Out_2;
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_a52c657743b446d58a731424c4c26be4_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_3de0ae68e22e43e5bcb14cec399e2036_Out_0 = IN.uv0;
            Bindings_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d _QTexture2D_1aab88b1d78a46708376aba33d58bb46;
            float4 _QTexture2D_1aab88b1d78a46708376aba33d58bb46_Output_1;
            float _QTexture2D_1aab88b1d78a46708376aba33d58bb46_R_2;
            float _QTexture2D_1aab88b1d78a46708376aba33d58bb46_G_3;
            float _QTexture2D_1aab88b1d78a46708376aba33d58bb46_B_4;
            float _QTexture2D_1aab88b1d78a46708376aba33d58bb46_A_5;
            SG_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d(float4 (1, 1, 0, 0), _Property_a52c657743b446d58a731424c4c26be4_Out_0, (_UV_3de0ae68e22e43e5bcb14cec399e2036_Out_0.xy), _QTexture2D_1aab88b1d78a46708376aba33d58bb46, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_Output_1, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_R_2, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_G_3, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_B_4, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_A_5);
            float4 _Property_87688a44688d4a73884f6af7ff3b0210_Out_0 = _MaskTillingOffset;
            UnityTexture2D _Property_052f19e5289e4f4da3bd7d9ec034e5a9_Out_0 = UnityBuildTexture2DStructNoScale(_Mask);
            Bindings_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9;
            float4 _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_Output_1;
            float _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_R_2;
            float _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_G_3;
            float _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_B_4;
            float _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_A_5;
            SG_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d(_Property_87688a44688d4a73884f6af7ff3b0210_Out_0, _Property_052f19e5289e4f4da3bd7d9ec034e5a9_Out_0, (_UV_3de0ae68e22e43e5bcb14cec399e2036_Out_0.xy), _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_Output_1, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_R_2, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_G_3, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_B_4, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_A_5);
            float _Property_a3b60728c57f4fbc950cdf30faff2013_Out_0 = _Reverse;
            float _OneMinus_a3f43e4368eb4c6abee69056827d82b1_Out_1;
            Unity_OneMinus_float(_Property_a3b60728c57f4fbc950cdf30faff2013_Out_0, _OneMinus_a3f43e4368eb4c6abee69056827d82b1_Out_1);
            float _Step_8cbd7126a9e44512b68c3499aeae6780_Out_2;
            Unity_Step_float(0.5, _OneMinus_a3f43e4368eb4c6abee69056827d82b1_Out_1, _Step_8cbd7126a9e44512b68c3499aeae6780_Out_2);
            float _Multiply_3e559c47d975410c9e062f1c9531eca2_Out_2;
            Unity_Multiply_float(_QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_A_5, _Step_8cbd7126a9e44512b68c3499aeae6780_Out_2, _Multiply_3e559c47d975410c9e062f1c9531eca2_Out_2);
            float _OneMinus_61ee9fd4b634444c91bf0197765407dd_Out_1;
            Unity_OneMinus_float(_QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_A_5, _OneMinus_61ee9fd4b634444c91bf0197765407dd_Out_1);
            float _Step_fe63526006e74359a8ede680a69156f4_Out_2;
            Unity_Step_float(0.5, _Property_a3b60728c57f4fbc950cdf30faff2013_Out_0, _Step_fe63526006e74359a8ede680a69156f4_Out_2);
            float _Multiply_fd779d831e4f49e9b71f624b1137a0a3_Out_2;
            Unity_Multiply_float(_OneMinus_61ee9fd4b634444c91bf0197765407dd_Out_1, _Step_fe63526006e74359a8ede680a69156f4_Out_2, _Multiply_fd779d831e4f49e9b71f624b1137a0a3_Out_2);
            float _Add_6c01540f94fd4c08a3b0a972f9726b32_Out_2;
            Unity_Add_float(_Multiply_3e559c47d975410c9e062f1c9531eca2_Out_2, _Multiply_fd779d831e4f49e9b71f624b1137a0a3_Out_2, _Add_6c01540f94fd4c08a3b0a972f9726b32_Out_2);
            float _Multiply_5856b1ec31c841839e44706f64022149_Out_2;
            Unity_Multiply_float(_Add_6c01540f94fd4c08a3b0a972f9726b32_Out_2, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_A_5, _Multiply_5856b1ec31c841839e44706f64022149_Out_2);
            surface.BaseColor = (_QTexture2D_1aab88b1d78a46708376aba33d58bb46_Output_1.xyz);
            surface.Alpha = _Multiply_5856b1ec31c841839e44706f64022149_Out_2;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.color = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _Mask_TexelSize;
        float4 _MaskTillingOffset;
        float _Reverse;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;
        TEXTURE2D(_Mask);
        SAMPLER(sampler_Mask);

            // Graph Functions
            
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Preview_float(float In, out float Out)
        {
            Out = In;
        }

        void Unity_Step_float2(float2 Edge, float2 In, out float2 Out)
        {
            Out = step(Edge, In);
        }

        void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        struct Bindings_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d
        {
        };

        void SG_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d(float4 Vector4_19C7703B, UnityTexture2D Texture2D_CA72CD38, float2 Vector2_F19B6F36, Bindings_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d IN, out float4 Output_1, out float R_2, out float G_3, out float B_4, out float A_5)
        {
            UnityTexture2D _Property_c20210f75e5841c8b4b7250cc2e4321e_Out_0 = Texture2D_CA72CD38;
            float2 _Property_10f7dba9ddb54aba84adb08a27e022b0_Out_0 = Vector2_F19B6F36;
            float4 _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0 = Vector4_19C7703B;
            float _Split_117dfb2a277c4a979671228912a2d2f1_R_1 = _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0[0];
            float _Split_117dfb2a277c4a979671228912a2d2f1_G_2 = _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0[1];
            float _Split_117dfb2a277c4a979671228912a2d2f1_B_3 = _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0[2];
            float _Split_117dfb2a277c4a979671228912a2d2f1_A_4 = _Property_15c92b40551a40d6a2f78b35caabd18e_Out_0[3];
            float2 _Vector2_9c22647769ea4c96af77199c7f700b00_Out_0 = float2(_Split_117dfb2a277c4a979671228912a2d2f1_R_1, _Split_117dfb2a277c4a979671228912a2d2f1_G_2);
            float2 _Vector2_729da08c140e4f0db6fd3c2399651a00_Out_0 = float2(_Split_117dfb2a277c4a979671228912a2d2f1_B_3, _Split_117dfb2a277c4a979671228912a2d2f1_A_4);
            float2 _TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3;
            Unity_TilingAndOffset_float(_Property_10f7dba9ddb54aba84adb08a27e022b0_Out_0, _Vector2_9c22647769ea4c96af77199c7f700b00_Out_0, _Vector2_729da08c140e4f0db6fd3c2399651a00_Out_0, _TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3);
            float4 _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c20210f75e5841c8b4b7250cc2e4321e_Out_0.tex, _Property_c20210f75e5841c8b4b7250cc2e4321e_Out_0.samplerstate, _TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3);
            float _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_R_4 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0.r;
            float _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_G_5 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0.g;
            float _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_B_6 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0.b;
            float _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_A_7 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0.a;
            float _Preview_49bd68e7bd714c78ab709ed19049b7ff_Out_1;
            Unity_Preview_float(_SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_A_7, _Preview_49bd68e7bd714c78ab709ed19049b7ff_Out_1);
            float2 _Step_74b80af085fb44dab793b8aebd02e674_Out_2;
            Unity_Step_float2(float2(0, 0), _TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3, _Step_74b80af085fb44dab793b8aebd02e674_Out_2);
            float2 _Step_cfc0107733af4fbab0e5b4ffd1a8c95b_Out_2;
            Unity_Step_float2(_TilingAndOffset_b05b63825f704a14b2b9d1484b21393e_Out_3, float2(1, 1), _Step_cfc0107733af4fbab0e5b4ffd1a8c95b_Out_2);
            float2 _Multiply_daacb5e105374afab1c4f7ac68f4c3ae_Out_2;
            Unity_Multiply_float(_Step_74b80af085fb44dab793b8aebd02e674_Out_2, _Step_cfc0107733af4fbab0e5b4ffd1a8c95b_Out_2, _Multiply_daacb5e105374afab1c4f7ac68f4c3ae_Out_2);
            float _Split_41b59f89add144378ab7e9985e715d68_R_1 = _Multiply_daacb5e105374afab1c4f7ac68f4c3ae_Out_2[0];
            float _Split_41b59f89add144378ab7e9985e715d68_G_2 = _Multiply_daacb5e105374afab1c4f7ac68f4c3ae_Out_2[1];
            float _Split_41b59f89add144378ab7e9985e715d68_B_3 = 0;
            float _Split_41b59f89add144378ab7e9985e715d68_A_4 = 0;
            float _Multiply_4a5f7cee40424bd4968573d70b63d22d_Out_2;
            Unity_Multiply_float(_Preview_49bd68e7bd714c78ab709ed19049b7ff_Out_1, _Split_41b59f89add144378ab7e9985e715d68_R_1, _Multiply_4a5f7cee40424bd4968573d70b63d22d_Out_2);
            float _Multiply_9c1d6ef3beeb45f9ae8fc5f2c61ead2a_Out_2;
            Unity_Multiply_float(_Multiply_4a5f7cee40424bd4968573d70b63d22d_Out_2, _Split_41b59f89add144378ab7e9985e715d68_G_2, _Multiply_9c1d6ef3beeb45f9ae8fc5f2c61ead2a_Out_2);
            Output_1 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_RGBA_0;
            R_2 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_R_4;
            G_3 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_G_5;
            B_4 = _SampleTexture2D_0c8b2fbf56fc4753ad68f3c49c1b895d_B_6;
            A_5 = _Multiply_9c1d6ef3beeb45f9ae8fc5f2c61ead2a_Out_2;
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_a52c657743b446d58a731424c4c26be4_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_3de0ae68e22e43e5bcb14cec399e2036_Out_0 = IN.uv0;
            Bindings_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d _QTexture2D_1aab88b1d78a46708376aba33d58bb46;
            float4 _QTexture2D_1aab88b1d78a46708376aba33d58bb46_Output_1;
            float _QTexture2D_1aab88b1d78a46708376aba33d58bb46_R_2;
            float _QTexture2D_1aab88b1d78a46708376aba33d58bb46_G_3;
            float _QTexture2D_1aab88b1d78a46708376aba33d58bb46_B_4;
            float _QTexture2D_1aab88b1d78a46708376aba33d58bb46_A_5;
            SG_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d(float4 (1, 1, 0, 0), _Property_a52c657743b446d58a731424c4c26be4_Out_0, (_UV_3de0ae68e22e43e5bcb14cec399e2036_Out_0.xy), _QTexture2D_1aab88b1d78a46708376aba33d58bb46, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_Output_1, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_R_2, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_G_3, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_B_4, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_A_5);
            float4 _Property_87688a44688d4a73884f6af7ff3b0210_Out_0 = _MaskTillingOffset;
            UnityTexture2D _Property_052f19e5289e4f4da3bd7d9ec034e5a9_Out_0 = UnityBuildTexture2DStructNoScale(_Mask);
            Bindings_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9;
            float4 _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_Output_1;
            float _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_R_2;
            float _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_G_3;
            float _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_B_4;
            float _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_A_5;
            SG_QTexture2D_76f4b07b850ad0a48ab4b91d1fa4734d(_Property_87688a44688d4a73884f6af7ff3b0210_Out_0, _Property_052f19e5289e4f4da3bd7d9ec034e5a9_Out_0, (_UV_3de0ae68e22e43e5bcb14cec399e2036_Out_0.xy), _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_Output_1, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_R_2, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_G_3, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_B_4, _QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_A_5);
            float _Property_a3b60728c57f4fbc950cdf30faff2013_Out_0 = _Reverse;
            float _OneMinus_a3f43e4368eb4c6abee69056827d82b1_Out_1;
            Unity_OneMinus_float(_Property_a3b60728c57f4fbc950cdf30faff2013_Out_0, _OneMinus_a3f43e4368eb4c6abee69056827d82b1_Out_1);
            float _Step_8cbd7126a9e44512b68c3499aeae6780_Out_2;
            Unity_Step_float(0.5, _OneMinus_a3f43e4368eb4c6abee69056827d82b1_Out_1, _Step_8cbd7126a9e44512b68c3499aeae6780_Out_2);
            float _Multiply_3e559c47d975410c9e062f1c9531eca2_Out_2;
            Unity_Multiply_float(_QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_A_5, _Step_8cbd7126a9e44512b68c3499aeae6780_Out_2, _Multiply_3e559c47d975410c9e062f1c9531eca2_Out_2);
            float _OneMinus_61ee9fd4b634444c91bf0197765407dd_Out_1;
            Unity_OneMinus_float(_QTexture2D_0ff6cdfc2e5c4af398833dffd3eaaeb9_A_5, _OneMinus_61ee9fd4b634444c91bf0197765407dd_Out_1);
            float _Step_fe63526006e74359a8ede680a69156f4_Out_2;
            Unity_Step_float(0.5, _Property_a3b60728c57f4fbc950cdf30faff2013_Out_0, _Step_fe63526006e74359a8ede680a69156f4_Out_2);
            float _Multiply_fd779d831e4f49e9b71f624b1137a0a3_Out_2;
            Unity_Multiply_float(_OneMinus_61ee9fd4b634444c91bf0197765407dd_Out_1, _Step_fe63526006e74359a8ede680a69156f4_Out_2, _Multiply_fd779d831e4f49e9b71f624b1137a0a3_Out_2);
            float _Add_6c01540f94fd4c08a3b0a972f9726b32_Out_2;
            Unity_Add_float(_Multiply_3e559c47d975410c9e062f1c9531eca2_Out_2, _Multiply_fd779d831e4f49e9b71f624b1137a0a3_Out_2, _Add_6c01540f94fd4c08a3b0a972f9726b32_Out_2);
            float _Multiply_5856b1ec31c841839e44706f64022149_Out_2;
            Unity_Multiply_float(_Add_6c01540f94fd4c08a3b0a972f9726b32_Out_2, _QTexture2D_1aab88b1d78a46708376aba33d58bb46_A_5, _Multiply_5856b1ec31c841839e44706f64022149_Out_2);
            surface.BaseColor = (_QTexture2D_1aab88b1d78a46708376aba33d58bb46_Output_1.xyz);
            surface.Alpha = _Multiply_5856b1ec31c841839e44706f64022149_Out_2;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            ENDHLSL
        }
    }
    CustomEditor "ad"
    FallBack "Hidden/Shader Graph/FallbackError"
}