import "reflect-metadata";


export interface IValidationRule {
  evaluate(target: any, value: any, key: string): string | null;
}

class RequiredValidationRule implements IValidationRule {
  static instance = new RequiredValidationRule();

  evaluate(target: any, value: any, key: string): string | null {
    if (value) {
      return null;
    }

    if (typeof value == "boolean" && value != null) {
      return null;
    }

    return `${key} is required`;
  }
}

class RangeValidationRule implements IValidationRule {
  constructor(private min: number, private max?: number) {

  }

  evaluate(target, value, key: string): string | null {
    if (typeof value !== "number") {
      return `${key} is not a number and cannot be validated as a range.`;
    }

    const valueNumber = value as number;
    if (valueNumber < this.min) {
      if (this.max > 0) {
        return `${key} is currently ${value}, must be between ${this.min} and ${this.max}.`;
      } else {
        return `${key} is currently ${value}, must be larger or equal to ${this.min}.`;
      }
    }

    if (this.max > 0 && valueNumber > this.max) {
      return `${key} is currently ${value}, must be between ${this.min} and ${this.max}.`;
    }

    return null;
  }
}

class StringLengthValidationRule implements IValidationRule {
  constructor(private max: number, private min?: number) {

  }

  evaluate(target, value, key: string): string | null {
    if (typeof value !== "string") {
      return `${key} is not a string and cannot be validated using StringLength.`;
    }

    const valueStr = value as string;
    if (!value || value.length === 0) {
      return null;
    }


    if (valueStr.length > this.max) {
      return `${key} must be at most ${this.min} characters.`;
    }

    if (this.min && valueStr.length < this.min) {
      return `${key} must be more than ${this.min} characters.`;
    }

    return null;
  }
}

export interface ICopyOptions {
  stringFields?: string[];
  numericFields?: string[];
  booleanFields?: string[];
  exclude?: string[];
  useDestinationProperties?: boolean;
  skipExistenceCheck?: boolean;
}

export function copy(source: any, destination: any, options?: ICopyOptions) {
  var propsObj = source;
  if (options && options.useDestinationProperties) {
    propsObj = destination;
  }

// ReSharper disable once MissingHasOwnPropertyInForeach
  for (let key in propsObj) {

    let canPass = options == null || (options.numericFields == null && options.stringFields == null && options.booleanFields == null);
    if (options) {
      if (options.numericFields && options.numericFields.includes(key)) {
        canPass = true;
      }
      if (options.stringFields && options.stringFields.includes(key)) {
        canPass = true;
      }
      if (options.booleanFields && options.booleanFields.includes(key)) {
        canPass = true;
      }
      if (options.exclude && options.exclude.includes(key)) {
        canPass = false;
      }

      if (!canPass) {
        continue;
      }
    }

    var skipCheck = options != null && options.skipExistenceCheck;
    if (!Object.prototype.hasOwnProperty.call(propsObj, key) && !skipCheck) {
      throw new Error(`Could not find '${key}', found fields: ${Object.keys(propsObj).join(', ')}`);
    }

    let value = source[key];
    if (options) {
      if (options.numericFields && options.numericFields.includes(key) && value != null) {
        value = parseInt(value);
      }
      if (options.booleanFields && options.booleanFields.includes(key) && value != null) {
        if (!value || value === "0" || value.toString().toLowerCase() === "false") {
          value = false;
        } else {
          value = true;
        }
      }
    }

    destination[key] = value;
  }

}

export function required(target: any, propertyKey: string) {
  addValidationRule(target, propertyKey, RequiredValidationRule.instance);
}


export function range(min: number, max?: number) {
  return (target: any, propertyKey: string) => {
    addValidationRule(target, propertyKey, new RangeValidationRule(min, max));
  }
}

export function range2(target: any, propertyKey: string, min: number, max: number) {
  return (target: any, propertyKey: string) => {
    addValidationRule(target, propertyKey, new RangeValidationRule(min, max));
  }
}


export function stringLength(max: number, min?: number) {
  return (target: any, propertyKey: string) => {
    addValidationRule(target, propertyKey, new StringLengthValidationRule(max, min));
  }
}


export function addValidationRule(target: any, propertyKey: string, rule: IValidationRule) {
  const rules: IValidationRule[] = Reflect.getMetadata("validation", target, propertyKey) || [];
  rules.push(rule);

  const properties: string[] = Reflect.getMetadata("validation", target) || [];
  if (properties.indexOf(propertyKey) < 0) {
    properties.push(propertyKey);
  }

  Reflect.defineMetadata("validation", properties, target);
  Reflect.defineMetadata("validation", rules, target, propertyKey);
}


export function validate(target: any) {
  // Get the list of properties to validate
  const keys = Reflect.getMetadata("validation", target) as string[];
  const errorMessages: string[] = [];
  if (Array.isArray(keys)) {
    for (const key of keys) {
      const rules = Reflect.getMetadata("validation", target, key) as IValidationRule[];
      if (!Array.isArray(rules)) {
        continue;
      }

      for (const rule of rules) {
        const error = rule.evaluate(target, target[key], key);
        if (error) {
          errorMessages.push(error);
        }
      }
    }
  }

  return errorMessages;
}

export function isValid(target: any) {
  const validationResult = validate(target);
  return validationResult.length === 0;
}
