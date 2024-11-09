import ApiUrls from "@/config/api-urls";
import * as signalR from "@microsoft/signalr";

export const connection = new signalR.HubConnectionBuilder()
    .withUrl(ApiUrls.signalR)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

export async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected");
    } catch (err) {
        console.error("Error connecting to SignalR:", err);
        setTimeout(startConnection, 5000);
    }
}