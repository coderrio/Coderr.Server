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
@Pipe({ name: 'isoDate' })
export class IsoDatePipe implements PipeTransform {
  transform(value: string | Date | null, format?: string): string {
    if (value === null || typeof value === "undefined") {
      return "n/a";
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

    if (format !== 'date') {
      return date.toLocaleString();
    } else {
      return date.toLocaleDateString();
    }

  }
}
