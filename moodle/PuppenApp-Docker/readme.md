# Docker deployment

```
# dir .
d-----        25.07.2020     00:20                build_image
d-----        25.07.2020     00:06                py_slim_django
d-----        24.07.2020     23:37                src
-a----        25.07.2020     01:13             21 .gitignore
-a----        25.07.2020     01:11            249 .gitmodules
-a----        24.07.2020     23:36            549 Dockerfile
-a----        25.07.2020     00:58           2356 image_manager.py
-a----        25.07.2020     01:01           2633 readme.md
```
>Dir-Listing

## Einführung
Im Grunde gibt es drei Container  - Einmal im Order build_image/Dockerfile(`build_image`), dann py_slim_django/Dockerfile(`py_slim`) und zuletzt im ./Dockerfile(`akte`)-Ordner.

`build_image` nutzt die git-submodule im selbigen Ordner um sowohl client als auch server zu Bauen und sie dem gesamtsystem bereitzustellen.
Die submodule müssen selbstständig aktuell gehalten werden und mit `manage_images.py -b build_image` neu erstellt und im system hinterlegt werden.

`py_slim` ist eine minimalversion basierend auf python3.7-slim. Installiert werden die notwendigen dependencies für django und apache2 zur auslieferung des clients im produktivbetrieb. Nachdem django und alle Abhängigkeiten gebaut und installiert  worden sind werden die Abhängigkeiten wieder gelöscht. (`py_slim_django/requirements.txt`)
Auch hier muss mit  `manage_images.py -b py_slim_django` neu gebaut werden.

`akte` ist die letzte image-Schicht, die die django-Instanz und die statischen client-Dateien gemeinsam über einen vhost ausrollt. Es werden die Dateien aus `src/` in die entsprechenden verzeichnisse im container kopiert. Die kompilierten Client und server-Dateien werden hier ebenso in das final image kopiert.

`run_app.py` ist der Einstiegspunkt der Gesamtanwendung, hier werden dann zuletzt Verzeichnissberechtigungen gesetzt, Umgebungsvariablen ausgelesen (`VIRTUAL_HOST`,`VIRTUAL_PORT`). Außerdem werden nach dem aktivieren der notwendigen module (`mod_rewrite`,`mod_cgi`) die Umgebungsvariablen über die  Config-Dateien(`/etc/apache2/envvars`, `/etc/apache2/ports.conf`) and apache übergeben. Zuletzt werden eventuell anstehende migrationen von django durchgeführt um dann apache mit allen neuen Konfigurationen neuzustarten.

## Management

`image_manager.py`

Das Script ist offensichtlich dazu die in `Einführung` Beschrieben stages zu bauen und mit docker bereitzustellen. Das ganze in Python um die plattformunabhängigkeit zu tackeln. So spart man einfach eine ;enge Building-Zeit wenn nicht jedes mal django oder der client komplett installiert und gebuildet werden müssen.

Änderungen am Code werden grundsätzlich in den entsprechenden Repos gemacht und dann wird in `build_image/client` und `build_image/server` entsprechend gepullt. Daraufhin wird (mindestens) das build_image mit `image_manager` neu gebaut.

```
# python3 image_maker.py --help

Docker Image Manager...
usage: image_manager.py [-h] [-b BUILD] [-r] [-s]

optional arguments:
  -h, --help            show this help message and exit
  -b BUILD, --build BUILD
                        image to rebuild [build_image|akte|py_slim_django|all]
  -r, --run             runs/restarts the image
  -s, --store           stores the image

```

> image_maker help screen