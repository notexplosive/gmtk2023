## Setup
- You'll need .NET6 to build the game
- If you're on mac or linux, shaders will not compile without wine. I have a `no-shaders` branch to mitigate this problem (this branch will not be up to date)

## To run the game:

```bash
git clone git@github.com:notexplosive/gmtk2023.git --init --recursive
cd gmtk2023

# runs the game in release mode
dotnet run --project GMTK23 --configuration Release
```