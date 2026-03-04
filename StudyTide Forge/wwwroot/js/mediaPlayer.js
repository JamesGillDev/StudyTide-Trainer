const VIDEO_EXTENSIONS = [".mp4", ".m4v", ".mov", ".avi", ".mkv", ".wmv", ".webm", ".mpeg", ".mpg"];
const MIME_BY_EXTENSION = {
    ".mp3": "audio/mpeg",
    ".wav": "audio/wav",
    ".ogg": "audio/ogg",
    ".m4a": "audio/mp4",
    ".aac": "audio/aac",
    ".flac": "audio/flac",
    ".wma": "audio/x-ms-wma",
    ".mp4": "video/mp4",
    ".m4v": "video/mp4",
    ".mov": "video/quicktime",
    ".avi": "video/x-msvideo",
    ".mkv": "video/x-matroska",
    ".wmv": "video/x-ms-wmv",
    ".webm": "video/webm",
    ".mpeg": "video/mpeg",
    ".mpg": "video/mpeg"
};

function inferVideoByExtension(fileName) {
    const lower = (fileName || "").toLowerCase();
    return VIDEO_EXTENSIONS.some(ext => lower.endsWith(ext));
}

function inferMimeByExtension(fileName) {
    const lower = (fileName || "").toLowerCase();
    const extension = Object.keys(MIME_BY_EXTENSION).find(ext => lower.endsWith(ext));
    return extension ? MIME_BY_EXTENSION[extension] : "";
}

export function collectSelectedFiles(input) {
    if (!input || !input.files) {
        return [];
    }

    const selected = [];

    for (const file of input.files) {
        const mimeType = file.type || inferMimeByExtension(file.name);
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

export async function setSourceAndPlay(media, sourceUrl, mimeType, volume) {
    if (!media || !sourceUrl) {
        return false;
    }

    media.pause();

    while (media.firstChild) {
        media.removeChild(media.firstChild);
    }

    const sourceElement = document.createElement("source");
    sourceElement.src = sourceUrl;
    if (mimeType) {
        sourceElement.type = mimeType;
    }

    media.appendChild(sourceElement);
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

export async function sourceExists(sourceUrl) {
    if (!sourceUrl) {
        return false;
    }

    if (sourceUrl.startsWith("blob:") || sourceUrl.startsWith("data:")) {
        return true;
    }

    try {
        const headResponse = await fetch(sourceUrl, {
            method: "HEAD",
            cache: "no-store"
        });

        if (headResponse.ok) {
            return true;
        }

        if (headResponse.status !== 405) {
            return false;
        }

        const getResponse = await fetch(sourceUrl, {
            method: "GET",
            cache: "no-store"
        });

        return getResponse.ok;
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
