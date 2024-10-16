try:
    from os import path
    import os, shutil

    dir_path = path.dirname(path.realpath(__file__))
    build_path = path.join(dir_path, "build")

    if path.isdir(build_path):
        shutil.rmtree(build_path)
    elif path.isfile(build_path):
        os.remove(build_path)

    os.makedirs(path.join(build_path, "data"))

    shutil.copyfile(path.join(dir_path, r"FolderOpener\folder.ico"), path.join(build_path, "folder.ico"))
    try:
        shutil.copyfile(path.join(dir_path, r"FolderOpener\bin\Release\FolderOpener.exe"), path.join(build_path, "FolderOpener.exe"))
        shutil.copyfile(path.join(dir_path, r"FolderCreater\bin\Release\FolderCreater.exe"), path.join(build_path, "FolderCreater.exe"))
        shutil.copyfile(path.join(dir_path, r"FolderCreater\bin\Release\ModernWpf.dll"), path.join(build_path, "ModernWpf.dll"))
        shutil.copyfile(path.join(dir_path, r"FolderCreater\bin\Release\ModernWpf.MessageBox.dll"), path.join(build_path, "ModernWpf.MessageBox.dll"))
    except Exception as e:
        print(e)
        raise Exception("Did you compile 'FolderCreater' and 'FolderOpener.exe' (for release)?")
except Exception as e:
    print(e)
