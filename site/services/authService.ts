const serviceAddress = `${process.env.SERVER_URL}authservice/`;

const AuthService = {
    async login(username: string, password: string) {
        const response = await fetch(`${serviceAddress}verify-password`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password }),
        });

        if (response.status === 400) {
            throw new Error('Wrong password or login.');
        }

        if (response.status >= 500) {
            throw new Error('Server error. Please try again later.');
        }

        if (!response.ok) {
            throw new Error('An unknown error occured.');
        }

        return await response.json();
    },
};

export default AuthService;