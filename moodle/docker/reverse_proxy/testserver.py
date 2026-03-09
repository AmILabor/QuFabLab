#!/usr/bin/env python
# Reflects the requests from HTTP methods GET, POST, PUT, and DELETE
# Written by Nathan Hamiel (2010)



import http.server
import socketserver
from argparse import ArgumentParser
import io
import sys


class EchoBoy(http.server.SimpleHTTPRequestHandler):
    def do_GET(self):
        request_path = self.path
        r = []
        enc = sys.getfilesystemencoding()
        r.append('<html>HALLO</html>')
        encoded = '\n'.join(r).encode(enc, 'surrogateescape')
        f = io.BytesIO()
        f.write(encoded)
        f.seek(0)
        self.send_response(200)
        self.send_header("Content-type", "text/html; charset=%s" % enc)
        self.send_header("Content-Length", str(len(encoded)))
        self.end_headers()
        return f



def main(args):
    port = args.port

    print('Listening on localhost:%s' % port)
    Handler = EchoBoy

    with socketserver.TCPServer(("", port), Handler) as httpd:
        print("serving at port", port)
        httpd.serve_forever()

if __name__ == "__main__":
    parser = ArgumentParser()
    parser.add_argument("--port","-port",type=int,required=True)
    args = parser.parse_args()
    main(args)