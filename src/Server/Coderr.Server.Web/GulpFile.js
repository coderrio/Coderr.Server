/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    sass = require("gulp-sass"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");


var config = {
    scripts: {
        bundle: 'bundle.min.js',
        dest: 'wwwroot/dist/js/',
        src: [
            'node_modules/jquery/dist/jquery.js',
            'node_modules/popper/dist/popper.js',
            'node_modules/bootstrap/dist/js/bootstrap.js'
        ],
        glob: 'wwwroot/js/**/*.js'
    },
    styles: {
        bundle: 'bundle.min.css',
        dest: 'wwwroot/dist/css/',
        src: [
            'wwwroot/dist/css/*.css',
            "!**/*.min.css"
        ]
    },
    sass: {
        dest: 'wwwroot/dist/css/',
        src: [
            'wwwroot/scss/*.scss',
            '!**/coderr-variables.scss'
        ],
        includePaths: ['./node_modules/bootstrap/scss']
    },
    fonts: {
        dest: 'wwwroot/dist/fonts/',
        src: ['wwwroot/fonts/**/*.{eot,svg,ttf,woff,woff2}']
    },
    distribution: 'wwwroot/dist/**/*'
};

function compileJs() {
    return gulp.src(config.scripts.src)
        .pipe(gulp.dest(config.scripts.dest));
}

function minifyJs() {
    return gulp.src(config.scripts.src, { base: "." })
        .pipe(concat(config.scripts.bundle))
        .pipe(uglify())
        .pipe(gulp.dest(config.scripts.dest));
}

function minifyCss() {
    return gulp.src(config.styles.src)
        .pipe(concat(config.styles.bundle))
        .pipe(cssmin())
        .pipe(gulp.dest(config.styles.dest));
}

function compileSass() {
    return gulp.src(config.sass.src)
        .pipe(sass({
            includePaths: config.sass.includePaths
        }))
        .pipe(gulp.dest(config.sass.dest));
}

function watchStyles() {
    return gulp.watch(config.sass.src, compileSass);
}


exports.minify = gulp.series(minifyJs, minifyCss);
exports.watch = watchStyles;
exports.default = gulp.series(compileSass, minifyCss);
