const path = require("path");
const webpackBaseConfig = require('./webpack.config.js');
const merge = require('webpack-merge');
const webpack = require("webpack");

const configDev = {
    mode: 'development',
    plugins: [
        new webpack.SourceMapDevToolPlugin({
            filename: "[file].map", // Remove this line if you prefer inline source maps
            moduleFilenameTemplate:
                path.relative("./wwwroot/dist",
                    "[resourcePath]") // Point sourcemap entries to the original file locations on disk
        })
    ]
};

module.exports = merge(webpackBaseConfig, configDev);