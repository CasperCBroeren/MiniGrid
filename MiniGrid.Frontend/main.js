const { app, BrowserWindow } = require('electron');
const path = require('path');

let mainWindow;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1024,
    height: 768,
    minWidth: 800,
    minHeight: 600,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
    },
  });

  // Determine if we're in development mode
  // Check if Vite dev server is running or if we're in a packaged app
  const isDev = process.env.NODE_ENV === 'development' || process.env.VITE_DEV_SERVER || !app.isPackaged;

  // In development, try to load from Vite dev server
  if (isDev) {
    // Try to connect to Vite dev server
    mainWindow.loadURL('http://localhost:3000');
    mainWindow.webContents.openDevTools();
    
    // Fallback if Vite isn't running
    mainWindow.webContents.on('did-fail-load', () => {
      console.log('Vite dev server not running, trying to load from disk...');
      const devIndex = path.join(process.cwd(), 'index.html');
      mainWindow.loadFile(devIndex);
    });
  } else {
    // Production: load from dist folder
    // When electron is launched from project root, dist is in ./dist
    // When packaged, resources are at process.resourcesPath
    const distPath = app.isPackaged 
      ? path.join(process.resourcesPath, 'dist', 'index.html')
      : path.join(process.cwd(), 'dist', 'index.html');
    
    console.log('Loading production build from:', distPath);
    mainWindow.loadFile(distPath);
  }

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', () => {
  if (mainWindow === null) {
    createWindow();
  }
});
