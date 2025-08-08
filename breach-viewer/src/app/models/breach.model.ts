export interface Breach {
  name: string;
  title: string;
  domain: string;
  addedDate: string;
  breachDate?: string;
  modifiedDate?: string;
  pwnCount: number;
  description: string;
  dataClasses: string[];
  isVerified: boolean;
  isFabricated: boolean;
  isSensitive: boolean;
  isRetired: boolean;
  isSpamList: boolean;
} 