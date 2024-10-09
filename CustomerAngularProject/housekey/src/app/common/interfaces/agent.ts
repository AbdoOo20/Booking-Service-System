export interface Agent {
    id: number;
    fullName: string;
    desc: string;
    organization :string;
    email: string;
    phone: string;
    social: Social;
    ratingsCount: number;
    ratingsValue: number;
    image: string;
}

interface Social { 
    facebook: string;
    twitter: string;
    linkedin :string;
    instagram: string;
    website: string;
}