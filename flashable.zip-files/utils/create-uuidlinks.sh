#!/sbin/sh

#create-uuidlinks.sh script by zxz0O0
#script to create links to partitions by gpt uuid
#the links will be deleted after reboot
#first parameter is working directory where sgdisk and busybox are located

if [ -n "$1" ];
then
	cd "$1"
fi

if [ ! -f sgdisk ];
then
	echo "Error: Could not find sgdisk"
	exit 1
fi

if [ ! -f busybox ];
then
	echo "Error: Could not find busybox"
	exit 1
fi

partitions=`./sgdisk -p /dev/block/mmcblk0 | ./busybox awk 'NR==4' | ./busybox awk '{ print $6 }'`
if [ "$partitions" = "" ];
then
	echo "Error: Could not get partitions"
	exit 2
fi
case $partitions in
	(*[^0-9]*|'')
		echo "Error: Partitions variable is not a number"
		exit 3
		;;
#	(*)
#		echo a number
#		;;
esac

mkdir -p /dev/block/platform/msm_sdcc.1/by-uuid
chmod 755 /dev/block/platform/msm_sdcc.1/by-uuid

i=1
while [ "$i" -le "$partitions" ];
do
	uuid=`./sgdisk -i $i /dev/block/mmcblk0 | ./busybox awk 'NR==2' | ./busybox awk '{ print $4 }'`
	if [ "$uuid" = "" ];
	then
		echo "Error: UUID is empty"
		exit 4
	fi
	pname="/dev/block/mmcblk0p$i"

	if [ ! -L "/dev/block/platform/msm_sdcc.1/by-uuid/$uuid" ];
	then
		ln -s "$pname" "/dev/block/platform/msm_sdcc.1/by-uuid/$uuid"
	fi

	i=$((i+1))
done

exit 0
