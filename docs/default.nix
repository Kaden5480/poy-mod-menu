let
    pkgs = import <nixpkgs> {};
in
    pkgs.mkShell {
        buildInputs = with pkgs; [
            dotnet-sdk
            dotnet-runtime
        ];

        shellHook = ''
        if [ ! -d .config ]; then
            dotnet new tool-manifest
            dotnet tool update --local docfx
        fi

        dotnet docfx
        exit
        '';
    }
