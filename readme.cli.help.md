## slndoc show-full-help
### Usage
  slndoc show-full-help `[options]`

### Options
 `-o|--output-markdown`  Output markdown file
  -?|-h|--help          Show help information.



## slndoc extract
Parses a solution (.sln) starting from api controllers and creates a dependency tree.

### Usage
  slndoc extract `[options]`

### Options
 `-i|--input-path`     The path to the sln file
 `-o|--output-path`    The path to the result file. The file type is given by one of the following extensions : json,
                      yaml|yml, mmd, svg
 `-r|--root-name`      [Required. the name of the root of the graph
 `-e|--external-only`  [Optional] (default : false). Determines if internal classes should be fetched (false) or not
                      (true)
 `-h|--hierarchical`   [Optional] (default: false) Determines if the export format is hierarchical or not. This option
                      has no impact on mermaid exrection.
  -?|--help           Show help information.



## slndoc merge
Merge two or more exported documents into one document.

### Usage
  slndoc merge `[options]` 	

### Arguments
  FilesPaths    The files to merge.

### Options
 `-o|--output`   The output file path for the merged document.
  -?|-h|--help  Show help information.



## slndoc export
Loads a non hierarchical json file and generates a mermaid file.

### Usage
  slndoc export `[options]`

### Options
 `-i|--input-path`   The path to the sln file
 `-o|--output-path`  The path to the resulting mermaid file.
  -?|-h|--help      Show help information.



## slndoc settings show
### Usage
  slndoc settings show `[options]`

### Options
  -?|-h|--help  Show help information.



## slndoc settings switchto
### Usage
  slndoc settings switchto `[options]` 	

### Arguments
  Environment   

### Options
  -?|-h|--help  Show help information.



## slndoc settings set exclude-classes
### Usage
  slndoc settings set exclude-classes `[options]` 	

### Arguments
  ExclusionRegexex  List of the names of the regexes to exclude class names
                    Example: 	 exclude-classes ^ILogger<.* ^string$

### Options
 `-e|--environment`  Environment to set the settings for
  -?|-h|--help      Show help information.



## slndoc settings set attributes-to-scan
### Usage
  slndoc settings set attributes-to-scan `[options]` 	

### Arguments
  Attributes        List of the names of the attributes that represent a dependency : Array of string in json format
                    Example: 	 attributes-to-scan attr1 attr2 attr3

### Options
 `-e|--environment`  Environment to set the settings for
  -?|-h|--help      Show help information.



## slndoc settings delete
### Usage
  slndoc settings delete `[options]` 	

### Arguments
  Environment   

### Options
  -?|-h|--help  Show help information.



## slndoc ping
### Usage
  slndoc ping `[options]`

### Options
  -?|-h|--help  Show help information.



