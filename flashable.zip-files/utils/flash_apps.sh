#!/sbin/sh

zip="$1"
if [ ! -f "$zip" ];
then
	echo "Error: Please specify a zip file as parameter"
	exit 1
fi

if [ ! -f busybox ];
then
	echo "Error: Could not find busybox"
	exit 1
fi

exitcode=1
folder=`pwd`

data_apps=`./busybox unzip -l "$zip" | grep "data/app/.*\.apk" | ./busybox awk '{ print $4 }'`
#data_apps_names=`echo "$data_apps" | ./busybox sed 's/data\/app\///g'`
echo $data_apps
#echo $data_apps_names

if [ -n "$data_apps" ];
then
	./busybox unzip "$zip" "$data_apps" -d /
	cd /
	chmod 644 $data_apps
	chown system:system $data_apps
	echo "Apps installed to /data"
	exitcode=0
fi

cd "$folder"

system_apps=`./busybox unzip -l "$zip" | grep "system/app/.*\.apk" | ./busybox awk '{ print $4 }'`
#system_apps_names=`echo "$system_apps" | ./busybox sed 's/system\/app\///g'`
echo $system_apps
#echo $system_apps_names

if [ -n "$system_apps" ];
then
	./busybox unzip "$zip" "$system_apps" -d /
	cd /
	chmod 644 $system_apps
	chown root:root $system_apps
	echo "Apps installed to /system"
	exitcode=0
fi

cd "$folder"

exit $exitcode