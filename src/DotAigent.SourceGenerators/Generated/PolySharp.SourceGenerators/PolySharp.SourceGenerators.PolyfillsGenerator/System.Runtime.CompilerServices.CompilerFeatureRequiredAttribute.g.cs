﻿// <auto-generated/>
#pragma warning disable
#nullable enable annotations

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Indicates that compiler support for a particular feature is required for the location where this attribute is applied.
    /// </summary>
    [global::System.AttributeUsage(global::System.AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class CompilerFeatureRequiredAttribute : global::System.Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="global::System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute"/> type.
        /// </summary>
        /// <param name="featureName">The name of the feature to indicate.</param>
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }

        /// <summary>
        /// The name of the compiler feature.
        /// </summary>
        public string FeatureName { get; }

        /// <summary>
        /// If true, the compiler can choose to allow access to the location where this attribute is applied if it does not understand <see cref="FeatureName"/>.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// The <see cref="FeatureName"/> used for the ref structs C# feature.
        /// </summary>
        public const string RefStructs = nameof(RefStructs);

        /// <summary>
        /// The <see cref="FeatureName"/> used for the required members C# feature.
        /// </summary>
        public const string RequiredMembers = nameof(RequiredMembers);
    }
}