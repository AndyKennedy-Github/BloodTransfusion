using System;
using UnityEngine;

namespace Moodle {

    [Serializable]
    public class Scorm {

        [SerializeField]
        private uint id;

        public uint Id {
            get { return id; }
        }

        [SerializeField]
        private uint coursemodule;

        public uint CourseModule {
            get { return coursemodule; }
        }

        [SerializeField]
        private uint course;

        public uint Course {
            get { return course; }
        }

        [SerializeField]
        private string name;

        public string Name {
            get { return name; }
        }

        [SerializeField]
        private string intro;

        public string Intro {
            get { return intro; }
        }

        [SerializeField]
        private uint introformat;

        public uint IntroFormat {
            get { return introformat; }
        }

        [SerializeField]
        private File[] introfiles;

        public File[] IntroFiles {
            get { return introfiles; }
        }

        [SerializeField]
        private uint packagesize;

        public uint PackageSize {
            get { return packagesize; }
        }

        [SerializeField]
        private string packageurl;

        public string PackageUrl {
            get { return packageurl; }
        }

        [SerializeField]
        private string version;

        public string Version {
            get { return version; }
        }

        [SerializeField]
        private uint maxgrade;

        public uint MaxGrade {
            get { return maxgrade; }
        }

        [SerializeField]
        private uint grademethod;

        public uint GradeMethod {
            get { return grademethod; }
        }

        [SerializeField]
        private uint whatgrade;

        public uint WhatGrade {
            get { return whatgrade; }
        }

        [SerializeField]
        private uint maxattempt;

        public uint MaxAttempt {
            get { return maxattempt; }
        }

        [SerializeField]
        private bool forcecompleted;

        public bool ForceCompleted {
            get { return forcecompleted; }
        }

        [SerializeField]
        private bool forcenewattempt;

        public bool ForceNewAttempt {
            get { return forcenewattempt; }
        }

        [SerializeField]
        private bool lastattemptlock;

        public bool LastAttemptLock {
            get { return lastattemptlock; }
        }

        [SerializeField]
        private uint displayattemptstatus;

        public uint DisplayAttemptStatus {
            get { return displayattemptstatus; }
        }

        [SerializeField]
        private bool displaycoursestructure;

        public bool DisplayCourseStructure {
            get { return displaycoursestructure; }
        }

        [SerializeField]
        private string sha1hash;

        public string Sha1Hash {
            get { return sha1hash; }
        }

        [SerializeField]
        private string md5hash;

        public string Md5Hash {
            get { return md5hash; }
        }

        [SerializeField]
        private uint revision;

        public uint Revision {
            get { return revision; }
        }

        [SerializeField]
        private uint launch;

        public uint Launch {
            get { return launch; }
        }

        [SerializeField]
        private uint skipview;

        public uint SkipView {
            get { return skipview; }
        }

        [SerializeField]
        private bool hidebrowse;

        public bool HideBrowse {
            get { return hidebrowse; }
        }

        [SerializeField]
        private uint hidetoc;

        public uint HideToc {
            get { return hidetoc; }
        }

        [SerializeField]
        private uint nav;

        public uint Nav {
            get { return nav; }
        }

        [SerializeField]
        private int navpositionleft;

        public int NavPositionLeft {
            get { return navpositionleft; }
        }

        [SerializeField]
        private int navpositiontop;

        public int NavPositionTop {
            get { return navpositiontop; }
        }

        [SerializeField]
        private bool auto;

        public bool Auto {
            get { return auto; }
        }

        [SerializeField]
        private uint popup;

        public uint Popup {
            get { return popup; }
        }

        [SerializeField]
        private uint width;

        public uint Width {
            get { return width; }
        }

        [SerializeField]
        private uint height;

        public uint Height {
            get { return height; }
        }

        [SerializeField]
        private uint timeopen;

        public uint TimeOpen {
            get { return timeopen; }
        }

        [SerializeField]
        private uint timeclose;

        public uint TimeClose {
            get { return timeclose; }
        }

        [SerializeField]
        private bool displayactivityname;

        public bool DisplayActivityName {
            get { return displayactivityname; }
        }

        [SerializeField]
        private string scormtype;

        public string ScormType {
            get { return scormtype; }
        }

        [SerializeField]
        private string reference;

        public string Reference {
            get { return reference; }
        }

        [SerializeField]
        private bool protectpackagedownloads;

        public bool ProtectPackageDownloads {
            get { return protectpackagedownloads; }
        }

    }

}