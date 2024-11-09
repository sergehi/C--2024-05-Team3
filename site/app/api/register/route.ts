import { NextResponse } from 'next/server';
import ApiUrls from '@/config/api-urls';

export async function POST(request: Request) {
    try {
        const body = await request.json();
        const response = await fetch(ApiUrls.authorizationService.register, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });

        if (response.status === 200) {
            return NextResponse.json({}, { status: 200 });
        }

        return NextResponse.json({}, { status: response.status });
    } catch (error) {
        return NextResponse.json({}, { status: 500 });
    }
}