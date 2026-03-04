const VIDEO_EXTENSIONS = [".mp4", ".m4v", ".mov", ".avi", ".mkv", ".wmv", ".webm", ".mpeg", ".mpg"];

function inferVideoByExtension(fileName) {
    const lower = (fileName || "").toLowerCase();
    return VIDEO_EXTENSIONS.some(ext => lower.endsWith(ext));
}

export function collectSelectedFiles(input) {
    if (!input || !input.files) {
        return [];
    }

    const selected = [];

    for (const file of input.files) {
        const mimeType = file.type || "";
        selected.push({
            name: file.name,
            mimeType,
            objectUrl: URL.createObjectURL(file),
            isVideo: mimeType.startsWith("video/") || inferVideoByExtension(file.name)
        });
    }

    return selected;
}

export function clearInput(input) {
    if (input) {
        input.value = "";
    }
}

export function revokeObjectUrl(url) {
    if (!url) {
        return;
    }

    URL.revokeObjectURL(url);
}

export function attachMediaHandlers(media, dotNetRef) {
    if (!media || !dotNetRef) {
        return;
    }

    media.onended = () => dotNetRef.invokeMethodAsync("HandleMediaEnded");
    media.onerror = () => dotNetRef.invokeMethodAsync("HandleMediaError");
}

export async function setSourceAndPlay(media, sourceUrl, volume) {
    if (!media || !sourceUrl) {
        return false;
    }

    media.src = sourceUrl;
    media.volume = normalizeVolume(volume);
    media.load();

    try {
        await media.play();
        return true;
    }
    catch {
        return false;
    }
}

export async function play(media) {
    if (!media) {
        return false;
    }

    try {
        await media.play();
        return true;
    }
    catch {
        return false;
    }
}

export function pause(media) {
    if (!media) {
        return;
    }

    media.pause();
}

export function setVolume(media, volume) {
    if (!media) {
        return;
    }

    media.volume = normalizeVolume(volume);
}

function normalizeVolume(volume) {
    if (typeof volume !== "number") {
        return 0.5;
    }

    if (Number.isNaN(volume)) {
        return 0.5;
    }

    return Math.min(1, Math.max(0, volume));
}
