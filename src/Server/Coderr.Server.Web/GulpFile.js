/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    sass = require("gulp-sass"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    gutil = require('gulp-util');


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

gulp.task("js", function () {
    return gulp.src(config.scripts.src)
        .pipe(gulp.dest(config.scripts.dest));
});

gulp.task("min:js", function () {
    return gulp.src(config.scripts.src, { base: "." })
        .pipe(concat(config.scripts.bundle))
        .pipe(uglify())
        .on('error', function (err) { gutil.log(gutil.colors.red('[Error]'), err.toString()); })
        .pipe(gulp.dest(config.scripts.dest));
});

gulp.task("min:css", function () {
    return gulp.src(config.styles.src)
        .pipe(concat(config.styles.bundle))
        .pipe(cssmin())
        .pipe(gulp.dest(config.styles.dest));
});

gulp.task("sass", function () {
    return gulp.src(config.sass.src)
        .pipe(sass({
            includePaths: config.sass.includePaths
        }))
        .pipe(gulp.dest(config.sass.dest));
});

gulp.task('watch', function () {
    gulp.watch(config.sass.src, ['sass']);
});

gulp.task("min", ["min:js", "min:css"]);
gulp.task("build", ['sass', /*'js', "min:js",*/ "min:css"]);
gulp.task('default', ['build']);