import { Pipe, PipeTransform } from '@angular/core';

/*
 * Display either a moment (.i.e. 5 days ago) or a specific date
 * depending on the amount of time that have passed since the date.
 *
 * Usage:
 *   value | moment:format:default
 * Example:
 *   {{ '2020-01-10 13:00' | moment:'date' }}
 *   formats to date only
*/
@Pipe({ name: 'ago' })
export class AgoPipe implements PipeTransform {
  transform(value: string | Date | null, defaultValue?: string, format?: string): string {
    if (value === null || typeof value === "undefined") {
      return defaultValue || "never";
    }

    let date = null;
    if (typeof value === "string") {

      // assume UTC if not specified.
      if (value.indexOf('+') === -1 && value.substr(-1, 1) !== 'Z') {
        value += 'Z';
      }

      date = new Date(value);
    } else {
      date = value;
    }

    const now = new Date();
    const diffSeconds = (now.getTime() - date.getTime()) / 1000;
    if (diffSeconds < 60) {
      return "a moment ago";
    }

    const diffMinutes = diffSeconds / 60;
    if (diffMinutes < 60) {
      if (diffMinutes === 1) {
        return "a minute ago";
      }
      return Math.round(diffMinutes) + " minutes ago";
    }

    const diffHours = diffSeconds / 60 / 60;
    if (diffHours < 60) {
      if (diffHours === 1) {
        return "an hour ago";
      }

      return Math.round(diffHours) + " hours ago";
    }


    const diffDays = diffSeconds / 60 / 60 / 24;
    if (diffDays < 6) {
      if (diffDays === 1) {
        return "a day ago";
      }

      return Math.round(diffDays) + " days ago";
    }

    if (format === 'full') {
      return date.toLocaleString();
    } else {
      return date.toLocaleDateString();
    }

  }
}
