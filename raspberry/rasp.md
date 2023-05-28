# Configurazione Raspberry Pi da riga di comando

Scaricare la ISO da [https://www.raspberrypi.com/software/operating-systems/](https://www.raspberrypi.com/software/operating-systems/)

```bash
cd ~/Scaricati
wget -c https://downloads.raspberrypi.org/raspios_lite_arm64/images/raspios_lite_arm64-2023-02-22/2023-02-21-raspios-bullseye-arm64-lite.img.xz
```

Quindi decomprimere l'archivio:

```bash
xz -d 2023-02-21-raspios-bullseye-arm64-lite.img.xz 
```
## Flashare il SO sulla microSD

Tramite il comando `lsblk -f` individuare il device corretto.

Quindi smontare il device. Una volta smontato procedere a:

- creare una nuova tabella gpt;
- creare una nuova partizione;
- formattare la partizione nel file-system `ext4`;
- flashare la iso tramite `dd`:

```bash
sudo lsblk -f
NAME                FSTYPE      FSVER LABEL  UUID                                 FSAVAIL FSUSE% MOUNTPOINTS
sda                                                                                              
├─sda1              vfat        FAT32 bootfs 37CA-39EC                                           
└─sda2              ext4        1.0   rootfs a4af13c6-d165-4cbd-a9f6-c961fef8255d  344,9M    73% /media/davide/rootfs

umount /dev/sda1
umount /dev/sda2

sudo fdisk /dev/sda
Comando (m per richiamare la guida): g
Created a new GPT disklabel (GUID: CE1B9100-FFF7-0041-9C6F-34080C8A08C6).
Comando (m per richiamare la guida): n
Numero della partizione (1-128, default 1): 
First sector (2048-124805086, default 2048): 
Last sector, +/-sectors or +/-size{K,M,G,T,P} (2048-124805086, default 124803071): 
Created a new partition 1 of type 'Linux filesystem' and of size 59,5 GiB.
Comando (m per richiamare la guida): w
The partition table has been altered.
Calling ioctl() to re-read partition table.
Syncing disks.

sudo mkfs.ext4 /dev/sda1
mke2fs 1.47.0 (5-Feb-2023)
Creating filesystem with 15600128 4k blocks and 3907584 inodes
Filesystem UUID: ab33b6d8-fdf0-44b5-977d-3511ee17c9ed
Superblock backups stored on blocks: 
        32768, 98304, 163840, 229376, 294912, 819200, 884736, 1605632, 2654208, 
        4096000, 7962624, 11239424

Allocating group tables: done                            
Writing inode tables: done                            
Creating journal (65536 blocks): done
Writing superblocks and filesystem accounting information: done   

umount /dev/sda1

dd if=2023-02-21-raspios-bullseye-arm64-lite.img of=/dev/sda status=progress

sudo lsblk
NAME                                          MAJ:MIN RM   SIZE RO TYPE  MOUNTPOINTS
sda                                             8:0    1  59,5G  0 disk  
├─sda1                                          8:1    1   256M  0 part  /media/davide/bootfs
└─sda2                                          8:2    1   1,7G  0 part  /media/davide/rootfs
```
In questo modo si otterranno le due partizioni seguenti:

```bash
sudo lsblk -f

NAME                FSTYPE      FSVER LABEL  UUID                                 FSAVAIL FSUSE% MOUNTPOINTS
sda                                                                                              
├─sda1              vfat        FAT32 bootfs 37CA-39EC                             224,4M    12% /media/davide/bootfs
└─sda2              ext4        1.0   rootfs a4af13c6-d165-4cbd-a9f6-c961fef8255d  344,9M    73% /media/davide/rootfs
```

## Configurazione della rete wireless

```bash
cd /media/davide/bootfs/

cat wpa_supplicant.conf 

ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev
country=IT
update_config=1

network={
 ssid="<name>"
 psk="***********"
}
```

## Configurazione utente

```bash
cat userconf.txt 

davide:$6$Nf4LCWz7u2fcO4Vs$Z2S/T6IjgLXWTAWZfmWUbnraE0haKD0O7tt43ytGC3O7sN14UDOOGeZ7RDHOUxUZlqOOq54Ry2eAVcNfnRbiX.
```
Per generare la password crittografata dare il comando:

```bash
openssl passwd -6
```

## Configurazione ssh

Creare in `bootfs` il file `.sh` e appendere `net.ifnames=0` nel file `cmdline.txt`

```bash
cat cmdline.txt 

console=serial0,115200 console=tty1 root=PARTUUID=e088fd39-02 rootfstype=ext4 fsck.repair=yes rootwait quiet init=/usr/lib/raspberrypi-sys-mods/firstboot net.ifnames=0
```
## Impostare indirizzo IP statico

```bash
cd /media/davide/rootfs/
cd etc/
tail -n 5 dhcpcd.conf 

interface wlan0
static ip_address=192.168.1.100/24
static routers=192.168.1.1
static domain_name_servers=5.2.75.75
```

## Cambiare hostname

Nella medesima directory, modificare il nome host nei due seguenti file:

```bash
cat hostname 

pi

cat hosts

127.0.0.1       localhost
::1             localhost ip6-localhost ip6-loopback
ff02::1         ip6-allnodes
ff02::2         ip6-allrouters

127.0.1.1               pi
```

## Collegamenti

- [https://www.raspberrypi.com/software/operating-systems/j](https://www.raspberrypi.com/software/operating-systems/j)
- [https://www.raspberrypi.com/documentation/computers/configuration.html#setting-up-a-headless-raspberry-pi](https://www.raspberrypi.com/documentation/computers/configuration.html#setting-up-a-headless-raspberry-pi)
- [https://learn.sparkfun.com/tutorials/headless-raspberry-pi-setup/ethernet-with-static-ip-address](https://learn.sparkfun.com/tutorials/headless-raspberry-pi-setup/ethernet-with-static-ip-address)
- [https://thepihut.com/blogs/raspberry-pi-tutorials/19668676-renaming-your-raspberry-pi-the-hostname](https://thepihut.com/blogs/raspberry-pi-tutorials/19668676-renaming-your-raspberry-pi-the-hostname)
