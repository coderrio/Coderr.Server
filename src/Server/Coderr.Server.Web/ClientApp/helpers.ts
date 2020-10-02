import { DateTime } from 'luxon';


export function getLuxonDate(valueUtc: string | Date): DateTime {
    if (typeof valueUtc === "string") {
        return DateTime.fromISO(valueUtc, { zone: "utc" }).toLocal();
    } else {
        return DateTime.fromJSDate(valueUtc).toLocal();
    }
}

export function isoDate(valueUtc: string | Date): string {
    return getLuxonDate(valueUtc).toISO({ suppressMilliseconds: true, includeOffset: false });
}

export function ago(valueUtc: string | Date): string {
    if (!valueUtc) return "n/a";

    return getLuxonDate(valueUtc).toRelative();
}

export function monthDay(valueUtc: string | Date): string {
    return getLuxonDate(valueUtc).toFormat("LLL d");
}

export function niceTime(valueUtc: string | Date): string {
    if (!valueUtc) return "n/a";
    return getLuxonDate(valueUtc).toLocaleString(DateTime.DATETIME_MED);
    //return moment.utc(value).local().format("LLLL");
}

export function agoOrDate(valueUtc: string | Date): string {
    var today = DateTime.local();
    console.log('ageOrDate', valueUtc);

    var reportDate = getLuxonDate(valueUtc);
    var diff = reportDate.diff(today, ['days', 'hours', 'minutes', 'seconds']);
    if (diff.days === 0 && diff.hours === 0 && diff.minutes === 0) {
        if (diff.seconds < 10) {
            return 'now';
        }
        return diff.seconds + " seconds ago";
    }

    console.log('DIFF', diff);
    if (!valueUtc) return "n/a";
    return reportDate.toRelative();
}

export function incidentState(state: string | number) {
    var stateNumber: number;
    if (typeof state === "string") {
        stateNumber = parseInt(state);
    } else {
        stateNumber = <number>state;
    }


    switch (stateNumber) {
        case 0:
            return "New";
        case 1:
            return "Assigned";
        case 2:
            return "Ignored";
        case 3:
            return "Closed";
    }
}