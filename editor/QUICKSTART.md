

### Integration Steps

1. **Generate your atlas** using the tool
2. **Copy the code snippet** from the generated .txt file
3. **Create a new C# script** or modify `symbols-texture-data.cs`
4. **Paste the code** into your SymbolsTextureData implementation
5. **Assign the texture** to your VFX Graph
6. **Test** with ToastSpawner

### File Locations

All files stay within the package directory:
```
Packages/iv.vfx.toast/
├── editor/
│   ├── font-atlas-generator.cs          ← The tool
│   └── README-FontAtlasGenerator.md     ← Full documentation
└── runtime/
    └── scripts/
        ├── symbols-texture-data.cs      ← Original example
        └── custom-symbols-texture-data.cs ← Template for custom atlases
```

### Tips

💡 **Character Size**: 64px is a good default  
💡 **Atlas Size**: Use 4x4 for up to 16 symbols  
💡 **Symbol Order**: Left→Right, Bottom→Top  
💡 **Monospace Fonts**: Give consistent spacing  
💡 **Font Texture**: Must be readable (the tool will prompt to enable if needed)

### Common Issues

⚠️ **Empty/Black Texture**: Font texture needs Read/Write enabled (tool will ask)  
⚠️ **Missing Characters**: Font doesn't contain the symbol - try another font  
⚠️ **Blurry Characters**: Increase Character Size or disable Anti-Aliasing  

### Common Symbols to Include

- **Numbers**: `0123456789`
- **Math**: `+-*/=`
- **Punctuation**: `.,!?`
- **Special**: `<>[](){}` 
- **Letters**: `ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`

### Need Help?

📖 See `README-FontAtlasGenerator.md` for full documentation  
📝 Check `custom-symbols-texture-data.cs` for code examples  
🎮 Test with the included `vfx-toast.vfx` sample

---

**Package**: iv.vfx.toast  
**Version**: Compatible with Unity 2021.3+  
**Dependencies**: Unity VFX Graph, TextMesh
