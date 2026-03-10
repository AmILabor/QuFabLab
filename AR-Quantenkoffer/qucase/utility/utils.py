import os


def generate_connect_qrcode(text: str):
    import io
    import qrcode
    text = text.strip()
    qr = qrcode.QRCode(version=4, box_size=2, border=2)
    qr.add_data(text)
    f = io.StringIO()
    qr.print_ascii(out=f,invert=True)
    f.seek(0)
    lines = f.readlines()
    _lines = ""
    for l in lines:
        _lines+=f"\t\t\t   {l}"
    return "\n"+_lines+ f"\n\t\t\t\t    {text}    "


def get_ip_port():
    ipv4 = os.popen(
        'ip addr show wlan0 | grep "\<inet\>" | awk \'{ print $2 }\' | awk -F "/" \'{ print $1 }\'').read().strip()
    return ipv4, 8123
