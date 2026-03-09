import argparse
import subprocess
import os
import time

def try_run(arg):
    print(f"Running: \t{' '.join(arg)}")
    try:
        subprocess.check_call(arg)
    except:
        pass

def refresh_akte():
    print(f"Refreshing {img_name} image.")
    try_run(["docker","kill",container_name])
    try_run(["docker","rm",container_name])
    try_run(["docker","rmi",img_name])
    subprocess.check_output(["docker","build","--progress=plain","--build-arg","GEN_CERTS=True","--no-cache","-t",img_name,"."])

def refresh_build_image():
    print("Refreshing build image.")
    try_run(["docker","rmi","moodle_api"])
    subprocess.check_output(["docker","build","--no-cache","-t","moodle_api","./build_image"])
    
def refresh_apache_py_slim():
    print("Refreshing apache_py_slim.")
    try_run(["docker","rmi","apache_py_slim"])
    subprocess.check_output(["docker","build","--no-cache","-t","apache_py_slim","./py_slim_django/apache_py_slim"])
def refresh_py_slim_django():
    print("Refreshing py_slim_django.")
    try_run(["docker","rmi","py_slim_django"])
    subprocess.check_output(["docker","build","--progress=plain","--no-cache","-t","py_slim_django","./py_slim_django"])

def run():
    print(f"Running {container_name} as container.")
    try_run(["docker","kill",container_name])
    try_run(["docker","rm",container_name])
    cmd_str = f"docker run -d -p 443:{vport} -e TERM=xterm --env VIRTUAL_PORT={vport} --env VIRTUALHOST={vhost} --env GEN_CERTS='True' --env DOMAIN=localhost --mount source=akte,target=/mnt --name {container_name} {img_name}"
    print(f"running {cmd_str}")
    subprocess.check_output(cmd_str.split(" "))

def run_builds(_build):
    if "," in _build:
        builds = args.build.split(",")
    else:
        builds = [args.build]
    for bkey in builds:
        if type(buildmap[bkey])==list:
            for _bk in buildmap[bkey]:
                timeit(buildmap[_bk])
        else:
            timeit(buildmap[bkey])

def store_akte():
    print(f"Storing {img_name} to {img_name}.tar")
    subprocess.check_output(["docker","save",img_name,"-o",f"{img_name}.tar"])


def timeit(f):
    st = time.time()
    f()
    print(f"In {time.time()-st}s\n")

vport = "443"
vhost = "localhost"
img_name="akte"
container_name="akte_1"
buildmap = {
    "py_slim_django":refresh_py_slim_django,
    img_name:refresh_akte,
    "all":["py_slim_django",img_name]
}

if __name__ == "__main__":
    print(" Docker Image Manager...\n","====="*10)

    parser = argparse.ArgumentParser()
    parser.add_argument("-b","--build",help=f"image to rebuild [{'|'.join(buildmap.keys())}] seperate by ','",type=str)
    parser.add_argument("-r","--run",help="runs/restarts the image",action="store_true")
    parser.add_argument("-s","--store",help="stores the image",action="store_true")
    args = parser.parse_args()
    if hasattr(args,"build"):
        run_builds(args.build)
    if args.run:
        timeit(run)
    if args.store:
        timeit(store_akte)
    
