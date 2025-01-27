from pathlib import Path
import shutil
import argparse
root_dir = Path(__file__).parent / "ailia-csharp" / "ailia-csharp" / "ailia"
api_dir = root_dir / "ailia-sdk-unity" / "Runtime" / "Api"
model_dir = root_dir / "ailia-sdk-unity" / "Runtime" / "Models"

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Extract Ailia's wrapper files for C#")
    parser.add_argument("target_dir", type=str, help="Path to the directory to generate wrapper files")
    args = parser.parse_args()

    target_dir = Path(args.target_dir)
    target_dir.mkdir(parents=True, exist_ok=True)

    for file in api_dir.glob("*.cs"):
        shutil.copy(str(file), target_dir)
    for file in model_dir.glob("*.cs"):
        if file.name == "AiliaLicense.cs":
            continue
        shutil.copy(str(file), target_dir)
    for file in root_dir.glob("*.cs"):
        shutil.copy(str(file), target_dir)
    print("Ailia's wrapper files are copied to", target_dir)
