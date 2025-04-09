# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## Unreleased

### Added

### Fixed

## 1.25.1501

### Added

- added support for *.gsb grid files
  
  - ``Proj4Net.Core.IO.Path.PROJ_LIB`` must be set to a path, where grid file is located
  - use paramter +nadgrids=filename.gsb
  - grids must be grids that transform from target to WGS84 (or some equivalent)

### Fixed

