#!/sbin/sh

if [ -n "$3" ];
then
	cd "$3"
fi

zip="$1"
OUTFD="$2"

ui_print() {
	echo -n -e "ui_print $1\n" > /proc/self/fd/$OUTFD
	echo -n -e "ui_print\n" > /proc/self/fd/$OUTFD
}

set_progress() {
	echo -n -e "set_progress $1\n" > /proc/self/fd/$OUTFD
}

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

if [ ! -f prfconfig ];
then
	echo "Error: Could not find prfconfig"
	exit 1
fi

if [ ! -f sinflash ];
then
	echo "Error: Could not find sinflash"
	exit 1
fi

sinfiles=`./busybox awk -F "=" '/sinfiles/ {print $2}' prfconfig`
#we can progress from 0.15 to 0.9 => 0.750000
#echo "sf: $sinfiles"
(
	export IFS=","
	#count system as double because of its size
	count=1
	curprogress=150000
	for file in $sinfiles; do
		if [ "$file" != "" ];
		then
			count=$((count+1))
		fi
	done
	
	if [ "$count" -lt "2" ];
	then
		exit 0
	fi
	
	step="$((750000 / $count))"
	for file in $sinfiles; do
		if [ "$file" = "" ];
		then
			continue
		fi

		#echo ./sinflash "${file}.sin" -z "$zip" -rfd "$OUTFD"
		ret=`./sinflash "${file}.sin" -z "$zip" -rfd "$OUTFD"`
		if [ "$ret" -ne "0" ];
		then
			ui_print "Error flashing ${file}.sin"
		fi
		
		curprogress="$(($curprogress + $step))"
		if [ "$file" = "system" ];
		then
			curprogress="$(($curprogress + $step))"
		fi

		set_progress "0.${curprogress}"
	done
)

exit 0
