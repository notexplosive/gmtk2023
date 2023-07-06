neato monogame-release-build .\GMTK23\GMTK23.csproj ".build"
powershell -command "ls .build | where name -like *.pdb | remove-item"
neato publish-itch ".build" "notexplosive" "gmtk2023" "windows"
explorer ".build"