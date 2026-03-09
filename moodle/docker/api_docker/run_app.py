import subprocess
import os
from time import sleep
from pprint import pprint

path ="/opt/api/manage.py"
arg0 ="runserver"
vp = os.environ.get("VIRTUAL_PORT")
if not vp:
    vp = "8080"
else:
    arg1 = "0.0.0.0:"+vp

vhost = os.environ.get("VIRTUAL_HOST")
if not vhost:
    vhost = "localhost"

print("VIRTUAL SETTINGS:",vp,vhost)

def print_line(fn,start,end):
    with open(fn,'r') as df:        
        lines = df.readlines()
    if end is None:
        pprint(lines[start:])
    if start is None:
        pprint(lines[:end])
    else:
        pprint(lines[start:end])

def checkset_config(fn,keyword,fmode="a"):
    with open(fn,'r') as df:
        check_txt = df.read()
    if keyword not in check_txt:
        print_line(fn,-2,None)
        with open(fn,fmode) as df:
            df.write("\n"+keyword+"\n")
        print_line(fn,-2,None)
keywords = [f"export VIRTUAL_PORT={vp}",f"export VIRTUAL_HOST={vhost}",f"Listen {vp}"]
files = ["/etc/apache2/envvars","/etc/apache2/envvars","/etc/apache2/ports.conf"]
modes = ["a","a","w","a","a"]

for i in range(len(keywords)):
    checkset_config(files[i],keywords[i],modes[i])
print("Starting subprocess calls...")
r = subprocess.run(['chown','www-data:','/mnt/db.sqlite3'])
r = subprocess.run(['chown','root:www-data','/mnt'])
r = subprocess.run(['chmod','774','/mnt'])
r = subprocess.run(['chown','-R','www-data:www-data','/mnt/media'])
r = subprocess.run(['chmod','-R','774','/mnt/media'])
r = subprocess.run(['a2enmod','wsgi'])
r = subprocess.run(['a2enmod','rewrite'])
#r = subprocess.run(['python', path, "makemigrations"])
#r = subprocess.run(['python', path, "migrate"])
r = subprocess.run(['python', path, "collectstatic","--noinput"])
r = subprocess.run(['service', 'apache2','restart'])
print("Service Restarted.")
while 1:
    bfn = "/var/log/apache2/"
    fns = [bfn+x for x in ["error.log","access.log","other_vhosts_access.log"]]
    while 1:
        os.system("clear")
        print("===="*8)
        for fn in fns:
            print(fn)
            print_line(fn,-10,None)
        print("===="*8)
        sleep(10)