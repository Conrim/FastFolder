try:
    from os import path
    import os, shutil

    dir_path = path.dirname(path.realpath(__file__))
    build_path = path.join(dir_path, "build")
    data_path = path.join(os.getenv('APPDATA'), "FastFolder_Conrim")

    if path.isdir(build_path):
        shutil.rmtree(build_path)
    elif path.isfile(build_path):
        os.remove(build_path)
    os.makedirs(build_path)

    try:
        shutil.rmtree(data_path)
    except:
        pass
    os.makedirs(data_path)

    shutil.copyfile(path.join(dir_path, r"FolderOpener\folder.ico"), path.join(build_path, "folder.ico"))
    try:
        shutil.copyfile(path.join(dir_path, r"FolderOpener\bin\Release\FolderOpener.exe"), path.join(build_path, "FolderOpener.exe"))
        shutil.copyfile(path.join(dir_path, r"FolderCreater\bin\Release\FolderCreater.exe"), path.join(build_path, "FolderCreater.exe"))
        shutil.copyfile(path.join(dir_path, r"FolderCreater\bin\Release\ModernWpf.dll"), path.join(build_path, "ModernWpf.dll"))
        shutil.copyfile(path.join(dir_path, r"FolderCreater\bin\Release\ModernWpf.MessageBox.dll"), path.join(build_path, "ModernWpf.MessageBox.dll"))
        print("build succesful (-:")
    except Exception as e:
        print(e)
        print()
        raise Exception("Did you compile 'FolderCreater' and 'FolderOpener.exe' (for release)?")
except Exception as e:
    print(e)
input("\nPress Enter to exit")
